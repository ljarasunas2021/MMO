﻿using System.Linq;
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

    [Header("Locomotion Blend Value Thresholds")]
    // locomotion blend tree threshold in between ...
    // idle and walk animation
    public float idleToWalkThreshold;
    // walk and run animation
    public float walkToRunThreshold;

    [Header("Speed Values")]
    // speed at which the player moves when 
    // running
    public float runSpeed;
    // walking
    public float walkSpeed;
    // rolling
    public float rollSpeed;
    // running Jump
    public float runningJumpSpeed;
    // walking Jump
    public float walkingJumpSpeed;
    // mid air
    public float midAirSpeed;

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

    // current speed of player
    private float currentSpeed;
    // y component of velocity of player
    private float velocityY;
    // value thats used as parameter in locomotion blend tree
    private float locomotionBlendVal;
    // current anim state - each anim state is assigned its own index and this is that index
    private States currentState;
    // previous anim state
    private States previousState;
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
        if (currentState != States.locomotion) return;
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
                if (locomotionBlendVal > walkToRunThreshold) targetSpeed = runSpeed;
                else if (locomotionBlendVal < walkToRunThreshold && locomotionBlendVal > idleToWalkThreshold) targetSpeed = walkSpeed;
                break;
            case States.fallToRoll:
                targetSpeed = rollSpeed;
                break;
            case States.walkingJump:
                targetSpeed = walkingJumpSpeed;
                break;
            case States.runningJump:
                targetSpeed = runningJumpSpeed;
                break;
            case States.defInAir:
                targetSpeed = midAirSpeed;
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
            if (locomotionBlendVal <= idleToWalkThreshold) SetCurrentState(States.boxJump);
            else if (locomotionBlendVal >= walkToRunThreshold) SetCurrentState(States.runningJump);
            else SetCurrentState(States.walkingJump);
        }

        Debug.Log(locomotionBlendVal + " " + Input.GetKey(KeyCode.Space));
    }

    /// <summary> Set the correct values if the player is in the air </summary>
    private void SetValuesIfMidAir()
    {
        // check if hitting anything and if so set current state appropriately
        float maxRaycastDownDist = new float[] { minDistFromGroundToBeMidAir, maxBoxJumpHeight, maxWalkingJumpHeight, maxRunningJumpHeight }.Max();
        RaycastHit hit;
        Ray ray = new Ray(transform.position + 2 * Vector3.up, Vector3.down);
        Physics.Raycast(ray, out hit, maxRaycastDownDist, 9);
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
        Debug.Log(state);
        previousState = currentState;
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
    public static string nextState = "NextState";
    public static string locomotionBlend = "LocomotionBlend";
}
#endregion

