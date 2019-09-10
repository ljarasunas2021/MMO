using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    public float gravity, runSpeed, walkSpeed, speedSmoothTime, turnSmoothTime;

    private float speedSmoothVelocity, turnSmoothVelocity, currentSpeed, velocityY;
    private Transform camTransform;
    private CharacterController characterController;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        camTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 inputDir = input.normalized;
        bool running = Input.GetKey(KeyCode.LeftShift);

        Move(inputDir, running);

        float animationSpeedPercent = (input != Vector2.zero) ? ((running) ? runSpeed : walkSpeed) : 0;
        animator.SetFloat("InputMagnitude", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    void Move(Vector2 inputDir, bool running)
    {
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        if (characterController.isGrounded) velocityY = 0;
        else velocityY += Time.deltaTime * gravity;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        characterController.Move(velocity * Time.deltaTime);
        currentSpeed = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
    }
}
