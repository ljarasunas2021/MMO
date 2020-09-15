using System.Linq;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Animator))]
///<summary> Script used to control the movement of the player </summary>
public class Movement : NetworkBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject hips;

    [Header("Locomotion Blend Values")]
    [SerializeField] private float idleVal;
    [SerializeField] private float walkVal;
    [SerializeField] private float runVal;

    [Header("Locomotion Blend Value Thresholds")]
    [SerializeField] private float idleToWalkThreshold;
    [SerializeField] private float walkToRunThreshold;

    [Header("Smooth Time Values")]
    [SerializeField] private float turnSmoothTime;
    [SerializeField] private float locomotionAccelerationSmoothTime;
    [SerializeField] private float locomotionDecelerationSmoothTime;

    [Header("Mid Air Values")]
    [SerializeField] private float minDistFromGroundToBeMidAir;
    [SerializeField] private float gravity;

    [Header("Landing Velocity Y's")]
    [SerializeField] private float softLandingMaxTimeInAir;
    [SerializeField] private float rollLandingMaxTimeInAir;

    [Header("Jump Maximum Distances From Ground")]
    [SerializeField] private float maxBoxJumpHeight;
    [SerializeField] private float maxWalkingJumpHeight;
    [SerializeField] private float maxRunningJumpHeight;

    [Header("Camera Values")]
    [SerializeField] public float camRotOffset;

    // smooth velocities
    private float speedSmoothVelocity;
    private float locomotionSmoothVelocity;
    private float turnSmoothVelocity;
    private float locomotionDirSmoothVelocity;

    // distance to raycast downwards
    private float maxRaycastDownDist;
    // current y velocity
    private float velocityY;
    // locmotion vars
    private float locomotionBlendVal;
    private float locomotionDirection;
    // current locomotion state
    [HideInInspector] public States currentState;
    // transform of camera
    private Transform camTransform;
    // animator of player
    private Animator animator;
    // character controller of player
    private CharacterController cc;
    // current camera mode
    private CameraModes currentCam;
    // displacement from gameobject pivot to hips
    private Vector3 toHipsDisplacement;
    // time in air
    private float timeInAir;
    // target rotation
    private float targetRotation;
    // player camera manager script
    private PlayerCameraManager playerCameraManager;

    // init vars
    private void Start()
    {
        animator = GetComponent<Animator>();
        camTransform = Camera.main.transform;
        currentState = 0;
        cc = GetComponent<CharacterController>();
        cc.enabled = true;
        maxRaycastDownDist = new float[] { minDistFromGroundToBeMidAir, maxBoxJumpHeight, maxWalkingJumpHeight, maxRunningJumpHeight }.Max();
        toHipsDisplacement = transform.position - hips.transform.position;
        playerCameraManager = GetComponent<PlayerCameraManager>();
    }

    // move correctly
    public void Move()
    {
        currentState = (States)animator.GetInteger(Parameters.currentState);

        Vector2 inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = inputVector.normalized;

        SetSpeed();

        SetLocomotionBlendValue(inputVector, Input.GetButton("Sprint"));

        RotatePlayer(inputDir, Input.GetButton("Free Rotate Camera"), Input.mousePosition);

        CheckForJump(inputVector, Input.GetButton("Jump"));

        SetValuesIfMidAir(Input.GetButton("Jump"));

        animator.SetInteger(Parameters.currentState, (int)currentState);
    }

    // move using wasd
    private void SetSpeed()
    {
        bool lockedCameraMode = playerCameraManager.ReturnCameraMode() == CameraModes.locked;
        Transform transformToUse = (lockedCameraMode) ? Camera.main.transform : transform;
        cc.Move(transformToUse.forward * animator.GetFloat(Parameters.currentSpeedZ) * Time.deltaTime);
        cc.Move(transformToUse.right * animator.GetFloat(Parameters.currentSpeedX) * Time.deltaTime);
    }

    // set locomotion blend and dir
    private void SetLocomotionBlendValue(Vector2 input, bool leftShift)
    {
        if (currentState != States.locomotion) return;

        float targetLocomotionBlendVal = 0;
        float targetLocomotionDirection = 0;

        if (UIManager.canMove)
        {
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
        }

        float locomotionDirSmoothTime = (targetLocomotionDirection - locomotionDirection > 0) ? locomotionAccelerationSmoothTime : locomotionDecelerationSmoothTime;
        locomotionDirection = Mathf.SmoothDamp(locomotionDirection, targetLocomotionDirection, ref locomotionDirSmoothVelocity, locomotionDirSmoothTime);

        float locomotionSmoothTime = (targetLocomotionBlendVal - locomotionBlendVal > 0) ? locomotionAccelerationSmoothTime : locomotionDecelerationSmoothTime;
        locomotionBlendVal = Mathf.SmoothDamp(locomotionBlendVal, targetLocomotionBlendVal, ref locomotionSmoothVelocity, locomotionSmoothTime);

        animator.SetFloat(Parameters.locomotionBlend, locomotionBlendVal);
        animator.SetFloat(Parameters.locomotionDir, locomotionDirection);
    }

    // rotate the player
    private void RotatePlayer(Vector2 inputDir, bool leftControl, Vector2 mousePos)
    {
        if (!UIManager.canMove) return;

        if (currentCam != CameraModes.locked)
        {
            if (inputDir != Vector2.zero && animator.GetBool(Parameters.canRotate))
            {
                if (!leftControl) targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, Camera.main.transform.eulerAngles.y - camRotOffset, 0));
        }
    }

    // check if jumping
    private void CheckForJump(Vector2 input, bool space)
    {
        if (!UIManager.canMove) return;

        if (space)
        {
            if (locomotionBlendVal <= idleToWalkThreshold) SetCurrentState(States.boxJump);
            else if (locomotionBlendVal >= walkToRunThreshold) SetCurrentState(States.runningJump);
            else SetCurrentState(States.walkingJump);
        }
    }

    // deal with in air movement
    private void SetValuesIfMidAir(bool space)
    {
        RaycastHit hit;
        Ray ray = new Ray(hips.transform.position, Vector3.down);
        Physics.Raycast(ray, out hit, maxRaycastDownDist, 1 << LayerMaskController.environment);

        if (hit.distance < minDistFromGroundToBeMidAir && hit.distance != 0)
        {
            if (currentState == States.defInAir)
            {
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

    private void SetCurrentState(States state) { currentState = state; }

    public void SetCurrentCam(CameraModes currentCam) { this.currentCam = currentCam; }
}

// anim states
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

// upper body states
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

// params in anim controller
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
    public static string leftFoot = "LeftFoot";
    public static string rightFoot = "RightFoot";
}
#endregion
