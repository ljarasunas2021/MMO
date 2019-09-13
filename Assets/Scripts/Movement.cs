using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
///<summary> Script used to control the movement of the player </summary>
public class Movement : MonoBehaviour
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

    [Header("Speed Values")]
    // speed at which the player moves when 
    // running
    public float runSpeed;
    // walking
    public float walkSpeed;
    // rolling
    public float rollSpeed;

    [Header("Smooth Time Values")]
    // time that it takes for the ____ value to go from its current value to its target value
    // turning 
    public float turnSmoothTime;
    // speed (but only used if value is getting large (i.e. accelerating))
    public float speedAccelerationSmoothTime;
    // speed (but only used if value is getting smaller(i.e. decelerating))
    public float speedDecelerationSmoothTime;
    // locomotion (but only used if value is getting large (i.e. accelerating))
    public float locomotionAccelerationSmoothTime;
    // locomotion (but only used if value is getting smaller(i.e. decelerating))
    public float locomotionDecelerationSmoothTime;

    [HeaderAttribute("Mid Air Values")]
    // minimum distance from the ground the player needs to be in order to play the mid air animation
    public float minDistFromGroundToBeMidAir;
    // gravity that interacts with the player
    public float gravity;

    // amount that ____ has moved towards its target value 
    // speed
    private float speedSmoothVelocity;
    // locomotion
    private float locomotionSmoothVelocity;
    // turning
    private float turnSmoothVelocity;

    // current speed of player
    private float currentSpeed;
    // y component of velocity of player
    private float velocityY;
    // value thats used as parameter in locomotion blend tree
    private float locomotionBlendVal;
    // current anim state - each anim state is assigned its own index and this is that index
    private States currentState;
    // transform of the camera
    private Transform camTransform;
    // player's character controller
    private CharacterController characterController;
    // player's animator
    private Animator animator;
    #endregion

    #region Initialize
    ///<summary> Initialize variables </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
        camTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
        currentState = 0;
    }
    #endregion

    #region CalledEveryFrame
    ///<summary> Takes care of things that should be called every frame </summary>
    void Update()
    {
        Move();
    }
    #endregion

    #region Movement
    /// <summary> All movement of the player is run through this void </summary>
    private void Move()
    {
        // get current state
        currentState = (States)animator.GetInteger("CurrentState");

        // find the input and a normalized input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        SetLocomotionBlendValue(input);

        RotatePlayer(inputDir);

        SetSpeed();

        CheckForJump(input);

        SetValuesIfMidAir();

        // set the current state to equal the appropriate currentState
        animator.SetInteger(Parameters.currentState, (int)currentState);
    }

    /// <summary> Set the correct locomotion blend value </summary>
    /// <param name = "input"> input found in update function </param>
    private void SetLocomotionBlendValue(Vector2 input)
    {
        // value that the locomotion blend value should be 
        float targetLocomotionBlendVal = 0;

        // set the target locomotion blend value
        if (input.x == 0 && input.y == 0) targetLocomotionBlendVal = idleVal;
        else if (Input.GetKey(KeyCode.LeftShift)) targetLocomotionBlendVal = runVal;
        else targetLocomotionBlendVal = walkVal;

        // set the locomotion bend value based on the locomotion smooth time - if that was in a phase of acceleration or deceleration
        float locomotionSmoothTime = (targetLocomotionBlendVal - locomotionBlendVal > 0) ? locomotionAccelerationSmoothTime : locomotionDecelerationSmoothTime;
        locomotionBlendVal = Mathf.SmoothDamp(locomotionBlendVal, targetLocomotionBlendVal, ref locomotionSmoothVelocity, locomotionSmoothTime);

        // set the locomotion blend parameter
        animator.SetFloat(Parameters.locomotionBlend, locomotionBlendVal);
    }

    ///<summary> Rotate the player accordingly </summary>
    ///<param name = "inputDir"> normalized input in the update function </param>
    private void RotatePlayer(Vector2 inputDir)
    {
        // if the input doesn't equal zero, player can rotate
        if (inputDir != Vector2.zero)
        {
            // find target rotation of player based on camera's transform and rotate towards that angle smoothly
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);
        }
    }

    ///<summary> Set the appropriate speed for the player </summary>
    private void SetSpeed()
    {
        // speed that player is trying to reach
        float targetSpeed = 0;

        // set target speed appropriately
        switch (currentState)
        {
            case States.locomotion:
                if (locomotionBlendVal == runVal) targetSpeed = runSpeed;
                else if (locomotionBlendVal == walkVal) targetSpeed = walkSpeed;
                break;
            case States.fallToRoll:
                targetSpeed = rollSpeed;
                break;
        }

        // set current speed appropriately based on target speed and if player is accelerating / decelerating
        float speedSmoothTime = (targetSpeed - currentSpeed > 0) ? speedAccelerationSmoothTime : speedDecelerationSmoothTime;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        // set the y component of the velocity based on the gravity
        if (characterController.isGrounded) velocityY = 0;
        else velocityY += Time.deltaTime * gravity;

        // move the character controller in the direction of the current speed and velocityY
        characterController.Move((transform.forward * currentSpeed + Vector3.up * velocityY) * Time.deltaTime);

        // set the current speed appropriately
        currentSpeed = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
    }

    ///<summary> Check if jump should be called </summary>
    ///<param name = "input"> input in the update function </param>
    private void CheckForJump(Vector2 input)
    {
        /// if space has been pressed jump
        if (Input.GetKey(KeyCode.Space))
        {
            /// base jump animation on the input
            if (input == Vector2.zero) currentState = States.boxJump;
            else currentState = States.runningJump;
        }
    }

    /// <summary> Set the correct values if the player is in the air </summary>
    private void SetValuesIfMidAir()
    {
        // check if hitting anything and if so set current state appropriately
        Ray ray = new Ray(transform.position + 2 * Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, minDistFromGroundToBeMidAir, 9)) { if (currentState == States.defInAir) { currentState = States.fallToRoll; } }
        else currentState = States.defInAir;
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
    fallToRoll = 6
}
#endregion

#region Parameters
/// <summary> Names of parameters used in the animation controller </summary>
public static class Parameters
{
    public static string currentState = "CurrentState";
    public static string locomotionBlend = "LocomotionBlend";
}
#endregion

