using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
///<summary> Script used to control the movement of the player </summary>
public class Movement : MonoBehaviour
{
    #region Variables
    [Header("Physics Based Movement")]
    public bool physicsBasedMovement;

    [Header("GameObjects")]
    public GameObject ragdoll;
    public GameObject ragdollHips;
    public GameObject nonRagdollHips;

    [Header("Locomotion Blend Values")]
    // locomotion blend tree value for a ...
    // idle animation
    [SerializeField] private float idleVal;
    // walk animation
    [SerializeField] private float walkVal;
    // run animation
    [SerializeField] private float runVal;

    [Header("Locomotion Blend Value Thresholds")]
    // locomotion blend tree threshold in between ...
    // idle and walk animation
    [SerializeField] private float idleToWalkThreshold;
    // walk and run animation
    [SerializeField] private float walkToRunThreshold;

    [Header("Smooth Time Values")]
    // time that it takes for the ____ value to go from its current value to its target value
    // turning 
    [SerializeField] private float turnSmoothTime;
    // speed (but only used if value is getting large (i.e. accelerating))
    [SerializeField] private float locomotionAccelerationSmoothTime;
    // locomotion (but only used if value is getting smaller(i.e. decelerating))
    [SerializeField] private float locomotionDecelerationSmoothTime;

    [Header("Mid Air Values")]
    // minimum distance from the ground the player needs to be in order to play the mid air animation
    [SerializeField] private float minDistFromGroundToBeMidAir;
    // gravity that interacts with the player
    [SerializeField] private float physicsGravity;
    [SerializeField] private float nonphysicsGravity;

    [Header("Landing Velocity Y's")]
    // for ___ landing, the velocityY must be less than that value
    // soft
    [SerializeField] private float softLandingMaxTimeInAir;
    // roll
    [SerializeField] private float rollLandingMaxTimeInAir;

    [Header("Jump Maximum Distances From Ground")]
    // max distance from ground before mid air animation plays for ____ animation
    // boxJump
    [SerializeField] private float maxBoxJumpHeight;
    // walking Jump
    [SerializeField] private float maxWalkingJumpHeight;
    // running Jump
    [SerializeField] private float maxRunningJumpHeight;

    [Header("Camera Values")]
    public float camRotOffset;


    // amount that ____ has moved towards its target value 
    // speed
    private float speedSmoothVelocity;
    // locomotion
    private float locomotionSmoothVelocity;
    // turning
    private float turnSmoothVelocity;
    private float locomotionDirSmoothVelocity;

    // distance raycast will go downwards
    private float maxRaycastDownDist;
    // y component of velocity of player
    private float velocityY;
    // value thats used as parameter in locomotion blend tree
    private float locomotionBlendVal;
    // value that used as direction parameter in locomotion blend tree
    private float locomotionDirection;
    // current anim state - each anim state is assigned its own index and this is that index
    [HideInInspector] public States currentState;
    // transform of the camera
    private Transform camTransform;
    // player's animator
    private Animator animator;
    // character controller
    private CharacterController cc;
    // current camera mode
    private CameraModes currentCam;
    // animator follow script
    private AnimatorFollow animatorFollow;
    // difference between non ragdoll and hips in pos
    private Vector3 nonRagdollToHipsPos;
    // total time in the air
    private float timeInAir;
    // if it is the local player
    private bool isLocalPlayer;
    // gravity to be used
    private float gravity;
    // target rotation of the player
    private float targetRotation;
    // hips gameobject to use
    private GameObject hips;
    // manager of the camera for the player
    private PlayerCameraManager playerCameraManager;
    #endregion

    #region Initialize

    ///<summary> Initialize variables </summary>
    private void Start()
    {
        animator = GetComponent<Animator>();
        camTransform = Camera.main.transform;
        currentState = 0;
        cc = GetComponent<CharacterController>();
        cc.enabled = true;
        maxRaycastDownDist = new float[] { minDistFromGroundToBeMidAir, maxBoxJumpHeight, maxWalkingJumpHeight, maxRunningJumpHeight }.Max();
        animatorFollow = ragdoll.GetComponent<AnimatorFollow>();
        nonRagdollToHipsPos = transform.position - nonRagdollHips.transform.position;
        isLocalPlayer = transform.root.GetComponent<BodyParts>().IsLocalPlayer();
        gravity = (physicsBasedMovement) ? physicsGravity : nonphysicsGravity;
        hips = (physicsBasedMovement) ? ragdollHips : nonRagdollHips;
        playerCameraManager = transform.parent.GetComponent<PlayerCameraManager>();
    }
    #endregion

    #region Movement

    /// <summary> All movement of the player is run through this void </summary>
    /// <param name = "input"> input struct </summary>
    public void Move()
    {
        // get current state
        currentState = (States)animator.GetInteger(Parameters.currentState);

        // find the input and a normalized input
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = inputVector.normalized;

        SetSpeed();

        SetLocomotionBlendValue(inputVector, Input.GetButton("Sprint"));

        RotatePlayer(inputDir, Input.GetButton("Free Rotate Camera"), Input.mousePosition);

        CheckForJump(inputVector, Input.GetButton("Jump"));

        SetValuesIfMidAir(Input.GetButton("Jump"));

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
        float targetLocomotionDirection = 0;

        bool lockedCameraMode = playerCameraManager.ReturnCameraMode() == CameraModes.locked;
        if (input.y == 0 && input.x == 0) targetLocomotionBlendVal = idleVal;
        else if (leftShift && ((lockedCameraMode && input.y != 0) || (!lockedCameraMode))) targetLocomotionBlendVal = runVal;
        else if ((lockedCameraMode && input.y != 0) || (!lockedCameraMode)) targetLocomotionBlendVal = walkVal;

        if (lockedCameraMode)
        {
            if (input.x != 0)
            {
                int dir = (input.x < 0) ? -1 : 1;
                if (leftShift) targetLocomotionDirection = dir * runVal;
                else targetLocomotionDirection = dir * walkVal;
            }

            if (input.y < 0) targetLocomotionBlendVal = -walkVal;
        }

        float locomotionDirSmoothTime = (targetLocomotionDirection - locomotionDirection > 0) ? locomotionAccelerationSmoothTime : locomotionDecelerationSmoothTime;
        locomotionDirection = Mathf.SmoothDamp(locomotionDirection, targetLocomotionDirection, ref locomotionDirSmoothVelocity, locomotionDirSmoothTime);

        // set the locomotion bend value based on the locomotion smooth time - if that was in a phase of acceleration or deceleration
        float locomotionSmoothTime = (targetLocomotionBlendVal - locomotionBlendVal > 0) ? locomotionAccelerationSmoothTime : locomotionDecelerationSmoothTime;
        locomotionBlendVal = Mathf.SmoothDamp(locomotionBlendVal, targetLocomotionBlendVal, ref locomotionSmoothVelocity, locomotionSmoothTime);

        // set the locomotion blend parameter
        animator.SetFloat(Parameters.locomotionBlend, locomotionBlendVal);
        animator.SetFloat(Parameters.locomotionDir, locomotionDirection);
    }

    ///<summary> Rotate the player accordingly </summary>
    ///<param name = "inputDir"> normalized input in the update function </param>
    /// <param name = "leftControl"> was left control pressed </param>
    private void RotatePlayer(Vector2 inputDir, bool leftControl, Vector2 mousePos)
    {
        if (currentCam != CameraModes.locked)
        {
            // if the input doesn't equal zero, player can rotate
            if (inputDir != Vector2.zero && animator.GetBool(Parameters.canRotate))
            {
                // find target rotation of player based on camera's transform and rotate towards that angle smoothly
                if (!leftControl) targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, Camera.main.transform.eulerAngles.y - camRotOffset, 0));
        }
    }

    ///<summary> Add speed to player </summary>
    private void SetSpeed()
    {
        bool lockedCameraMode = playerCameraManager.ReturnCameraMode() == CameraModes.locked;
        Transform transformToUse = (lockedCameraMode) ? Camera.main.transform : transform;
        cc.Move(transformToUse.forward * animator.GetFloat(Parameters.currentSpeedZ) * Time.deltaTime);
        cc.Move(transformToUse.right * animator.GetFloat(Parameters.currentSpeedX) * Time.deltaTime);
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
    /// <param name = "space"> if the player presses space </param>
    private void SetValuesIfMidAir(bool space)
    {
        RaycastHit hit;
        Ray ray = new Ray(hips.transform.position, Vector3.down);
        Physics.Raycast(ray, out hit, maxRaycastDownDist, 1 << LayerMaskController.environment);

        if (hit.distance < minDistFromGroundToBeMidAir && hit.distance != 0)
        {
            if (currentState == States.defInAir)
            {
                if (physicsBasedMovement)
                {
                    animatorFollow.ChangeCurrentAnim(animatorFollow.locomotionAnim);
                    cc.Move(ragdollHips.transform.position + nonRagdollToHipsPos - transform.position);
                }

                if (timeInAir < softLandingMaxTimeInAir) SetCurrentState(States.softLanding);
                else if (timeInAir < rollLandingMaxTimeInAir) SetCurrentState(States.fallToRoll);
                else SetCurrentState(States.hardLanding);
            }
            timeInAir = 0;
        }
        else
        {
            if (currentState != States.defInAir)
            {
                if (physicsBasedMovement) animatorFollow.ChangeCurrentAnim(animatorFollow.fallingAnim);
                SetCurrentState(States.defInAir);
            }
            timeInAir += Time.deltaTime;
        }

        if (cc.isGrounded) velocityY = 0;
        else velocityY += gravity * Time.deltaTime;

        cc.Move(Vector3.up * velocityY);

        if (cc.isGrounded) velocityY = 0;

        if (currentState == States.boxJump && (hit.distance > maxBoxJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);
        else if (currentState == States.walkingJump && (hit.distance > maxWalkingJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);
        else if (currentState == States.runningJump && (hit.distance > maxRunningJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);
    }

    ///<summary> Set the current and previous state to their corresponding values </summary>
    ///<param name = "stateIndex"> Index of state that the current state should be equal to </param>
    private void SetCurrentState(States state) { currentState = state; }
    #endregion

    #region SetCurrentCam
    ///<summary> set the current camera </summary>
    ///<param name = "currentCam"> current camera </param>
    public void SetCurrentCam(CameraModes currentCam) { this.currentCam = currentCam; }
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
    walkingJump = 7,
    getUpFront = 8,
    getUpFrontMirror = 9,
    getUpBack = 10,
    getUpBackMirror = 11,
    knockedOut = 12
}

public enum UpperBodyStates
{
    none = 0,
    pistolHold = 1,
    shotgunHold = 2,
    swordHold = 3,
    midInwardSlashRight = 4,
    midSlashLeft = 5,
    highToLowInwardSlashRight = 6,
    lowToHighSlashLeft = 7,
    highToLowSlashLeft = 8,
    lowToHighInwardSlashRight = 9
}
#endregion

#region Parameters
/// <summary> Names of parameters used in the animation controller </summary>
public static class Parameters
{
    public static string currentState = "CurrentState";
    public static string currentSpeedX = "CurrentSpeedX";
    public static string currentSpeedZ = "CurrentSpeedZ";
    public static string locomotionBlend = "LocomotionBlend";
    public static string canRotate = "CanRotate";
    public static string upperBodyState = "UpperBodyState";
    public static string targetUpperBodyState = "TargetUpperBodyState";
    public static string locomotionDir = "LocomotionDir";
    public static string knockedOut = "KnockedOut";
}
#endregion
