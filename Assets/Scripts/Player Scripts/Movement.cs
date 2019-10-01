using System.Linq;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
///<summary> Script used to control the movement of the player </summary>
public class Movement : NetworkBehaviour
{
    #region Variables
    [Header("Locomotion Blend Values")]
    // locomotion blend tree value for a ...
    // idle animation
    public float idleVal;
    // walk animation
    public float walkVal;
    // run animation
    public float runVal;

    [Header("Locomotion Blend Value Thresholds")]
    // locomotion blend tree threshold in between ...
    // idle and walk animation
    public float idleToWalkThreshold;
    // walk and run animation
    public float walkToRunThreshold;

    [Header("Smooth Time Values")]
    // time that it takes for the ____ value to go from its current value to its target value
    // turning 
    public float turnSmoothTime;
    // speed (but only used if value is getting large (i.e. accelerating))
    public float locomotionAccelerationSmoothTime;
    // locomotion (but only used if value is getting smaller(i.e. decelerating))
    public float locomotionDecelerationSmoothTime;

    [Header("Mid Air Values")]
    // minimum distance from the ground the player needs to be in order to play the mid air animation
    public float minDistFromGroundToBeMidAir;
    // gravity that interacts with the player
    public float gravity;

    [Header("Landing Velocity Y's")]
    // for ___ landing, the velocityY must be less than that value
    // soft
    public float softLandingMaxVeloY;
    // roll
    public float rollLandingMaxVeloY;

    [Header("Jump Maximum Distances From Ground")]
    // max distance from ground before mid air animation plays for ____ animation
    // boxJump
    public float maxBoxJumpHeight;
    // walking Jump
    public float maxWalkingJumpHeight;
    // running Jump
    public float maxRunningJumpHeight;

    // amount that ____ has moved towards its target value 
    // speed
    private float speedSmoothVelocity;
    // locomotion
    private float locomotionSmoothVelocity;
    // turning
    private float turnSmoothVelocity;

    // distance raycast will go downwards
    private float maxRaycastDownDist;
    // y component of velocity of player
    private float velocityY;
    // target y rotation of player
    private float targetRotation;
    // value thats used as parameter in locomotion blend tree
    private float locomotionBlendVal;
    // current anim state - each anim state is assigned its own index and this is that index
    private States currentState;
    // transform of the camera
    private Transform camTransform;
    // player's character controller
    public CharacterController characterController;
    // player's animator
    private Animator animator;
    private UIManager UIScript;
    #endregion

    // private bool togglePauseMenu;

    #region Initialize

    ///<summary> Initialize variables </summary>
    private void Start()
    {
        characterController.enabled = true;
        UIScript = GameObject.Find("UI Manager").GetComponent<UIManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator = GetComponent<Animator>();
        camTransform = Camera.main.transform;
        currentState = 0;
        maxRaycastDownDist = new float[] { minDistFromGroundToBeMidAir, maxBoxJumpHeight, maxWalkingJumpHeight, maxRunningJumpHeight }.Max();
    }
    #endregion

    #region Update
    ///<summary> Takes care of things that should be called every frame </summary>
    void Update()
    {
        if (!isLocalPlayer) return;
        
        if (Input.GetKeyDown("p")) {
            UIScript.SendMessage("TogglePauseMenu");
            if (UIScript.togglePauseMenu) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        if (!UIScript.togglePauseMenu) {
            InputStruct input = new InputStruct(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), Input.GetKey(KeyCode.Space), Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftControl), camTransform.eulerAngles.y);
            Move(input);
        }
    }
    #endregion

    #region Movement

    /// <summary> All movement of the player is run through this void </summary>
    /// <param name = "input"> input struct </summary>
    private void Move(InputStruct input)
    {
        // get current state
        currentState = (States)animator.GetInteger(Parameters.currentState);

        // find the input and a normalized input
        Vector2 inputVector = new Vector2(input.horAxis, input.vertAxis);
        Vector2 inputDir = inputVector.normalized;

        SetSpeed();

        SetLocomotionBlendValue(inputVector, input.leftShift);

        RotatePlayer(inputDir, input.leftControl, input.camYRot);

        AddGravity(input.space);

        CheckForJump(inputVector, input.space);

        SetValuesIfMidAir();

        // set the current state to equal the appropriate currentState
        animator.SetInteger(Parameters.currentState, (int)currentState);
    }

    /// <summary> Set the correct locomotion blend value </summary>
    /// <param name = "input"> input found in update function </param>
    /// <param name = "leftShift"> was left shift pressed </param>
    private void SetLocomotionBlendValue(Vector2 input, bool leftShift)
    {

        if (currentState != States.locomotion) return;
        // value that the locomotion blend value should be 
        float targetLocomotionBlendVal = 0;

        // set the target locomotion blend value
        if (input.x == 0 && input.y == 0) targetLocomotionBlendVal = idleVal;
        else if (leftShift) targetLocomotionBlendVal = runVal;
        else targetLocomotionBlendVal = walkVal;

        // set the locomotion bend value based on the locomotion smooth time - if that was in a phase of acceleration or deceleration
        float locomotionSmoothTime = (targetLocomotionBlendVal - locomotionBlendVal > 0) ? locomotionAccelerationSmoothTime : locomotionDecelerationSmoothTime;
        locomotionBlendVal = Mathf.SmoothDamp(locomotionBlendVal, targetLocomotionBlendVal, ref locomotionSmoothVelocity, locomotionSmoothTime);

        // set the locomotion blend parameter
        animator.SetFloat(Parameters.locomotionBlend, locomotionBlendVal);
    }

    ///<summary> Rotate the player accordingly </summary>
    ///<param name = "inputDir"> normalized input in the update function </param>
    /// <param name = "leftControl"> was left control pressed </param>
    /// <param name = "camYRot"> what is the y rotation of the player </param>
    private void RotatePlayer(Vector2 inputDir, bool leftControl, float camYRot)
    {
        // if the input doesn't equal zero, player can rotate
        if (inputDir != Vector2.zero && animator.GetBool(Parameters.canRotate))
        {
            // find target rotation of player based on camera's transform and rotate towards that angle smoothly
            if (!leftControl) targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camYRot;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
    }

    /// <summary> Add appropriate gravity </summary>
    /// <param name = "space"> if space was pressed </param>
    private void AddGravity(bool space)
    {
        if (characterController.isGrounded && !space) velocityY = 0;
        else velocityY += Time.deltaTime * gravity;

        characterController.Move(Vector3.up * velocityY);
    }

    ///<summary> Add speed to player </summary>
    private void SetSpeed()
    {
        characterController.Move(transform.forward * animator.GetFloat(Parameters.currentSpeed) * Time.deltaTime);
    }

    ///<summary> Check if jump should be called </summary>
    ///<param name = "input"> input in the update function </param>
    /// <param name = "space"> was the space bar pressed </param>
    private void CheckForJump(Vector2 input, bool space)
    {
        /// if space has been pressed jump
        if (space)
        {
            /// base jump animation on the input
            if (locomotionBlendVal <= idleToWalkThreshold) SetCurrentState(States.boxJump);
            else if (locomotionBlendVal >= walkToRunThreshold) SetCurrentState(States.runningJump);
            else SetCurrentState(States.walkingJump);
        }
    }

    /// <summary> Set the correct values if the player is in the air </summary>
    private void SetValuesIfMidAir()
    {
        // check if hitting anything and if so set current state appropriately
        RaycastHit hit;
        Ray ray = new Ray(transform.position + 2 * Vector3.up, Vector3.down);
        Physics.Raycast(ray, out hit, maxRaycastDownDist, GetComponent<FootIK>().environment);
        if (hit.distance < minDistFromGroundToBeMidAir && hit.distance != 0)
        {
            if (currentState == States.defInAir)
            {
                if (velocityY > softLandingMaxVeloY) SetCurrentState(States.softLanding);
                else if (velocityY > rollLandingMaxVeloY) SetCurrentState(States.fallToRoll);
                else SetCurrentState(States.hardLanding);
            }
        }
        else if (currentState != States.defInAir) SetCurrentState(States.defInAir);

        if (currentState == States.boxJump && (hit.distance > maxBoxJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);

        if (currentState == States.walkingJump && (hit.distance > maxWalkingJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);

        if (currentState == States.runningJump && (hit.distance > maxRunningJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);
    }

    ///<summary> Set the current and previous state to their corresponding values </summary>
    ///<param name = "stateIndex"> Index of state that the current state should be equal to </param>
    private void SetCurrentState(States state)
    {
        currentState = state;
    }
    #endregion
}

#region States
///<summary> Animations and indexes associated with those animations </summary>
public enum States
{
    locomotion = 0,
    boxJump = 1,
    runningJump = 2,
    defInAir = 3,
    hardLanding = 4,
    softLanding = 5,
    fallToRoll = 6,
    walkingJump = 7
}
#endregion

#region Parameters
/// <summary> Names of parameters used in the animation controller </summary>
public static class Parameters
{
    public static string currentState = "CurrentState";
    public static string currentSpeed = "CurrentSpeed";
    public static string locomotionBlend = "LocomotionBlend";
    public static string canRotate = "CanRotate";
}
#endregion

#region  InputStruct
/// <summary> Struct where input is stored </summary>
public struct InputStruct
{
    public float horAxis;
    public float vertAxis;
    public bool space;
    public bool leftShift;
    public bool leftControl;
    public float camYRot;

    public InputStruct(float horAxis, float vertAxis, bool space, bool leftShift, bool leftControl, float camYRot)
    {
        this.horAxis = horAxis;
        this.vertAxis = vertAxis;
        this.space = space;
        this.leftShift = leftShift;
        this.leftControl = leftControl;
        this.camYRot = camYRot;
    }
}
#endregion