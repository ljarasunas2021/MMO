using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    #region Variables
    [Header("Locomotion Values")]
    public float idleVal;
    public float walkVal, runVal;

    [Header("Speed Values")]
    public float runSpeed;
    public float walkSpeed, rollSpeed;

    [Header("Smooth Time Values")]
    public float speedAccelerationSmoothTime;
    public float speedDecelerationSmoothTime, turnSmoothTime, locomotionAccelerationSmoothTime, locomotionDecelerationSmoothTime;

    [HeaderAttribute("Mid Air Values")]
    public float minDistFromGroundToBeMidAir;
    public float gravity;

    private float speedSmoothVelocity, locomotionSmoothVelocity, turnSmoothVelocity, currentSpeed, velocityY, locomotionBlendVal;
    private States currentState;
    private Transform camTransform;
    private CharacterController characterController;
    private Animator animator;
    #endregion

    #region Initialize
    void Start()
    {
        animator = GetComponent<Animator>();
        camTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
        currentState = 0;
    }
    #endregion

    #region CalledEveryFrame
    void Update()
    {
        Move();
    }
    #endregion

    #region Movement
    private void Move()
    {
        currentState = (States)animator.GetInteger("CurrentState");

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        SetLocomotionBlendValue(input);

        RotatePlayer(inputDir);

        SetSpeed();

        CheckForJump(input);

        SetValuesIfMidAir();

        animator.SetInteger(Parameters.currentState, (int)currentState);
    }

    private void SetLocomotionBlendValue(Vector2 input)
    {
        float targetLocomotionBTVal = 0;

        if (input.x == 0 && input.y == 0) targetLocomotionBTVal = idleVal;
        else if (Input.GetKey(KeyCode.LeftShift)) targetLocomotionBTVal = runVal;
        else targetLocomotionBTVal = walkVal;

        float locomotionSmoothTime = (targetLocomotionBTVal - locomotionBlendVal > 0) ? locomotionAccelerationSmoothTime : locomotionDecelerationSmoothTime;
        locomotionBlendVal = Mathf.SmoothDamp(locomotionBlendVal, targetLocomotionBTVal, ref locomotionSmoothVelocity, locomotionSmoothTime);

        animator.SetFloat(Parameters.locomotionBlend, locomotionBlendVal);
    }

    private void RotatePlayer(Vector2 inputDir)
    {
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
    }

    private void SetSpeed()
    {
        float targetSpeed = 0;
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

        float speedSmoothTime = (targetSpeed - currentSpeed > 0) ? speedAccelerationSmoothTime : speedDecelerationSmoothTime;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        if (characterController.isGrounded) velocityY = 0;
        else velocityY += Time.deltaTime * gravity;

        characterController.Move((transform.forward * currentSpeed + Vector3.up * velocityY) * Time.deltaTime);
        currentSpeed = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
    }

    private void CheckForJump(Vector2 input)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (input == Vector2.zero) currentState = States.boxJump;
            else currentState = States.runningJump;
        }
    }

    private void SetValuesIfMidAir()
    {
        float? hitDist = null;

        RaycastHit hit;
        Ray ray = new Ray(transform.position + 2 * Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 9)) hitDist = hit.distance;

        if (hitDist == null || hitDist > minDistFromGroundToBeMidAir) currentState = States.defInAir;
        else if (currentState == States.defInAir) currentState = States.fallToRoll;
    }
    #endregion
}

#region States
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
public static class Parameters
{
    public static string currentState = "CurrentState";
    public static string locomotionBlend = "LocomotionBlend";
}
#endregion

