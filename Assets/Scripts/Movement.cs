using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum states
{
    locomotion = 0,
    boxJump = 1,
    runningJump = 2,
    defInAir = 3,
    hardLanding = 4,
    softLanding = 5,
    fallToRoll = 6
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    public float idleVal, walkVal, runVal, minDistFromGroundToBeMidAir, gravity, runSpeed, walkSpeed, rollSpeed, speedAccelerationSmoothTime, speedDecelerationSmoothTime, turnSmoothTime, locomotionAccelerationSmoothTime, locomotionDecelerationSmoothTime;

    private states currentState;
    private float speedSmoothVelocity, locomotionSmoothVelocity, turnSmoothVelocity, currentSpeed, velocityY, locomotionBTVal;
    private Transform camTransform;
    private CharacterController characterController;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        camTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
        currentState = 0;
    }

    void Update()
    {
        currentState = (states)animator.GetInteger("CurrentState");

        Debug.Log(currentState);

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;

        float targetLocomotionBTVal = 0;
        if (input.x == 0 && input.y == 0) targetLocomotionBTVal = idleVal;
        else if (Input.GetKey(KeyCode.LeftShift)) targetLocomotionBTVal = runVal;
        else targetLocomotionBTVal = walkVal;

        float locomotionSmoothTime = (targetLocomotionBTVal - locomotionBTVal > 0) ? locomotionAccelerationSmoothTime : locomotionDecelerationSmoothTime;
        locomotionBTVal = Mathf.SmoothDamp(locomotionBTVal, targetLocomotionBTVal, ref locomotionSmoothVelocity, locomotionSmoothTime);

        Move(inputDir);

        if (Input.GetKey(KeyCode.Space))
        {
            if (input == Vector2.zero) currentState = states.boxJump;
            else currentState = states.runningJump;
        }

        float? hitDist = null;

        RaycastHit hit;
        Ray ray = new Ray(transform.position + 2 * Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 9)) hitDist = hit.distance;

        if (hitDist == null || hitDist > minDistFromGroundToBeMidAir) currentState = states.defInAir;
        else if (currentState == states.defInAir) currentState = states.fallToRoll;

        animator.SetInteger("CurrentState", (int)currentState);
        animator.SetFloat("LocomotionBT", locomotionBTVal);
    }

    void Move(Vector2 inputDir)
    {
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        float targetSpeed = 0;
        switch (currentState)
        {
            case states.locomotion:
                if (locomotionBTVal == runVal) targetSpeed = runSpeed;
                else if (locomotionBTVal == walkVal) targetSpeed = walkSpeed;
                break;
            case states.fallToRoll:
                targetSpeed = rollSpeed;
                break;
        }

        float speedSmoothTime = (targetSpeed - currentSpeed > 0) ? speedAccelerationSmoothTime : speedDecelerationSmoothTime;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        //Debug.Log(currentSpeed + " " + targetSpeed + " " + currentState + " " + locomotionBTVal);

        if (characterController.isGrounded) velocityY = 0;
        else velocityY += Time.deltaTime * gravity;

        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        characterController.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
    }
}

