﻿using System.Linq;
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
    // speed that the locked cam moves at
    public float lockedCamRotSpeed;

    [Header("Mid Air Values")]
    // minimum distance from the ground the player needs to be in order to play the mid air animation
    public float minDistFromGroundToBeMidAir;
    // gravity that interacts with the player
    public float gravity;
    // hieght of player
    public float height;
    // sphere overlap radius
    public float sphereOverlapRadius;

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
    // target y rotation of player
    private float targetRotation;
    // value thats used as parameter in locomotion blend tree
    private float locomotionBlendVal;
    // value that used as direction parameter in locomotion blend tree
    private float locomotionDirection;
    // current anim state - each anim state is assigned its own index and this is that index
    [HideInInspector] public States currentState;
    // transform of the camera
    private Transform camTransform;
    // player's character controller
    private CharacterController characterController;
    // player's animator
    private Animator animator;
    // player's ragdoll controller
    private RagdollController ragdollController;
    // player health script
    private PlayerHealth playerHealth;
    // player camera manager script
    private PlayerCameraManager playerCameraManager;
    // current camera
    private CameraModes currentCam = CameraModes.cinematic;
    private UIManager uIScript;
    private bool canMove = true;
    #endregion

    #region Initialize

    ///<summary> Initialize variables </summary>
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterController.enabled = true;
        animator = GetComponent<Animator>();
        ragdollController = GetComponent<RagdollController>();
        playerHealth = GetComponent<PlayerHealth>();
        playerCameraManager = GetComponent<PlayerCameraManager>();
        uIScript = GameObject.FindObjectOfType<UIManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        camTransform = Camera.main.transform;
        currentState = 0;
        maxRaycastDownDist = new float[] { minDistFromGroundToBeMidAir, maxBoxJumpHeight, maxWalkingJumpHeight, maxRunningJumpHeight }.Max();
    }
    #endregion

    #region Movement

    /// <summary> All movement of the player is run through this void </summary>
    /// <param name = "input"> input struct </summary>
    public void Move(InputStruct input)
    {
        // get current state
        currentState = (States)animator.GetInteger(Parameters.currentState);

        // find the input and a normalized input
        Vector2 inputVector = new Vector2(input.horAxis, input.vertAxis);
        Vector2 inputDir = inputVector.normalized;

        SetSpeed();

        SetLocomotionBlendValue(inputVector, input.sprint);

        RotatePlayer(inputDir, input.freeRotateCamera, input.mousePos);

        CheckForJump(inputVector, input.jump);

        SetValuesIfMidAir(input.jump);

        // set the current state to equal the appropriate currentState
        animator.SetInteger(Parameters.currentState, (int)currentState);
    }

    /// <summary> Set the correct locomotion blend value </summary>
    /// <param name = "input"> input found in update function </param>
    /// <param name = "leftShift"> was left shift pressed </param>
    private void SetLocomotionBlendValue(Vector2 input, bool leftShift)
    {
        if (currentState != States.locomotion) return;

        if (!uIScript.canMove)
        {
            input = Vector2.zero;
            leftShift = false;
        }

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
        if (!uIScript.canMove) return;

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
        characterController.Move(transformToUse.forward * animator.GetFloat(Parameters.currentSpeedZ) * Time.deltaTime);
        characterController.Move(transformToUse.right * animator.GetFloat(Parameters.currentSpeedX) * Time.deltaTime);
    }

    ///<summary> Check if jump should be called </summary>
    ///<param name = "input"> input in the update function </param>
    /// <param name = "space"> was the space bar pressed </param>
    private void CheckForJump(Vector2 input, bool space)
    {
        if (!uIScript.canMove) return;

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
        Ray ray = new Ray(transform.position + 2 * Vector3.up, Vector3.down);
        Physics.Raycast(ray, out hit, maxRaycastDownDist, 1 << LayerMaskController.environment);

        if (hit.distance < minDistFromGroundToBeMidAir && hit.distance != 0)
        {
            if (currentState == States.defInAir)
            {
                if (velocityY > softLandingMaxVeloY) SetCurrentState(States.softLanding);
                else if (velocityY > rollLandingMaxVeloY) SetCurrentState(States.fallToRoll);
                else SetCurrentState(States.hardLanding);
            }
        }
        else if (currentState != States.defInAir && velocityY != 0) { SetCurrentState(States.defInAir); }

        if (velocityY < -0.4 && hit.distance != 0)
        {
            playerHealth.SubtractHealth(100);
            ragdollController.CmdBecomeRagdoll();
        }

        if (characterController.isGrounded) velocityY = 0;
        else velocityY += Time.deltaTime * gravity;

        characterController.Move(Vector3.up * velocityY);

        if (characterController.isGrounded) velocityY = 0;

        if (currentState == States.boxJump && (hit.distance > maxBoxJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);
        else if (currentState == States.walkingJump && (hit.distance > maxWalkingJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);
        else if (currentState == States.runningJump && (hit.distance > maxRunningJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);
    }

    ///<summary> Set the current and previous state to their corresponding values </summary>
    ///<param name = "stateIndex"> Index of state that the current state should be equal to </param>
    private void SetCurrentState(States state) { currentState = state; }
    #endregion

    #region DrawGizmos
    ///<summary> draw gizmos to show overlapping sphere </summary>
    private void OnDrawGizmos() { Gizmos.DrawSphere(transform.position, sphereOverlapRadius); }
    #endregion

    #region SetCurrentCam
    ///<summary> set the current camera </summary>
    ///<param name = "currentCam"> current camera </param>
    public void SetCurrentCam(CameraModes currentCam) { this.currentCam = currentCam; }
    #endregion

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }
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