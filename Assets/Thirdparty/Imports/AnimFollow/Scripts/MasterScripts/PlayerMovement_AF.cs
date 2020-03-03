using UnityEngine;
using System.Linq;
using System.Collections;

namespace AnimFollow
{
    public class PlayerMovement_AF : MonoBehaviour
    {
        Animator anim;          // Reference to the animator component.
        HashIDs_AF hash;            // Reference to the HashIDs.

        public float animatorSpeed = 1.3f; // Read by RagdollControl
        public float speedDampTime = .1f;   // The damping for the speed parameter
        float mouseInput;
        public float mouseSensitivityX = 100f;
        public bool inhibitMove = false; // Set from RagdollControl

        [HideInInspector] public bool inhibitRun = false; // Set from RagdollControl

        public float idleVal;
        public float walkVal;
        public float runVal;

        public float idleToWalkThreshold, walkToRunThreshold;

        public float turnSmoothTime;
        public float locomotionAccelerationSmoothTime;
        public float locomotionDecelerationSmoothTime;

        public float minDistFromGroundToBeMidAir;

        public float maxBoxJumpHeight;
        public float maxWalkingJumpHeight;
        public float maxRunningJumpHeight;

        private float speedSmoothVelocity;
        private float locomotionSmoothVelocity;
        private float turnSmoothVelocity;
        private float locomotionDirSmoothVelocity;

        private float targetRotation;

        private float maxRaycastDownDist;

        private Animator animator;
        [HideInInspector] public States currentState = 0;
        private float locomotionDirection, locomotionBlendVal;

        private Transform camTransform;

        void Awake()
        {
            CheckForComponents();

            animator = GetComponent<Animator>();
            camTransform = Camera.main.transform;
            maxRaycastDownDist = new float[] { minDistFromGroundToBeMidAir, maxBoxJumpHeight, maxWalkingJumpHeight, maxRunningJumpHeight }.Max();
        }

        void OnAnimatorMove()
        {
            transform.position += anim.deltaPosition;
        }


        void FixedUpdate()
        {
            if (inhibitMove) return;

            Move();
        }

        private void Move()
        {
            currentState = (States)animator.GetInteger(Parameters.currentState);

            Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector2 inputDir = inputVector.normalized;

            //SetSpeed();

            SetLocomotionBlendValue(inputVector, Input.GetButton("Sprint"));

            RotatePlayer(inputDir, Input.GetButton("Free Rotate Camera"), Input.mousePosition);

            CheckForJump(inputVector, Input.GetButton("Jump"));

            SetValuesIfMidAir(Input.GetButton("Jump"));

            animator.SetInteger(Parameters.currentState, (int)currentState);
        }

        /// <summary> Set the correct locomotion blend value </summary>
        /// <param name = "input"> input found in update function </param>
        /// <param name = "leftShift"> was left shift pressed </param>
        private void SetLocomotionBlendValue(Vector2 input, bool leftShift)
        {
            if (currentState != States.locomotion) return;

            /*if (!uIScript.canMove)
            {
                input = Vector2.zero;
                leftShift = false;
            }*/

            float targetLocomotionBlendVal = 0;
            float targetLocomotionDirection = 0;

            bool lockedCameraMode = false;//playerCameraManager.ReturnCameraMode() == CameraModes.locked;
            if (input.y == 0 && input.x == 0) targetLocomotionBlendVal = idleVal;
            else if (leftShift && !inhibitRun && ((lockedCameraMode && input.y != 0) || (!lockedCameraMode))) targetLocomotionBlendVal = runVal;
            else if ((lockedCameraMode && input.y != 0) || (!lockedCameraMode)) targetLocomotionBlendVal = walkVal;

            /*if (lockedCameraMode)
            {
                if (input.x != 0)
                {
                    int dir = (input.x < 0) ? -1 : 1;
                    if (leftShift) targetLocomotionDirection = dir * runVal;
                    else targetLocomotionDirection = dir * walkVal;
                }

                if (input.y < 0) targetLocomotionBlendVal = -walkVal;
            }*/

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
            //if (!uIScript.canMove) return;

            // if (currentCam != CameraModes.locked)
            // {
            // if the input doesn't equal zero, player can rotate
            if (inputDir != Vector2.zero && animator.GetBool(Parameters.canRotate))
            {
                // find target rotation of player based on camera's transform and rotate towards that angle smoothly
                if (!leftControl) targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }
            // }
            // else
            // {
            //     transform.rotation = Quaternion.Euler(new Vector3(0, Camera.main.transform.eulerAngles.y - camRotOffset, 0));
            // }
        }

        /*private void SetSpeed()
        {
            //bool lockedCameraMode = playerCameraManager.ReturnCameraMode() == CameraModes.locked;
            Transform transformToUse = transform; //(lockedCameraMode) ? Camera.main.transform : transform;
            characterController.Move(transformToUse.forward * animator.GetFloat(Parameters.currentSpeedZ) * Time.deltaTime);
            characterController.Move(transformToUse.right * animator.GetFloat(Parameters.currentSpeedX) * Time.deltaTime);
        }*/

        private void CheckForJump(Vector2 input, bool space)
        {
            //if (!uIScript.canMove) return;

            if (space)
            {
                if (locomotionBlendVal <= idleToWalkThreshold) SetCurrentState(States.boxJump);
                else if (locomotionBlendVal >= walkToRunThreshold) SetCurrentState(States.runningJump);
                else SetCurrentState(States.walkingJump);
            }
        }

        private void SetValuesIfMidAir(bool space)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + 2 * Vector3.up, Vector3.down);
            Physics.Raycast(ray, out hit, maxRaycastDownDist, 1 << LayerMaskController.environment);

            if (hit.distance < minDistFromGroundToBeMidAir && hit.distance != 0)
            {
                if (currentState == States.defInAir)
                {
                    //if (velocityY > softLandingMaxVeloY) SetCurrentState(States.softLanding);
                    /*else if (velocityY > rollLandingMaxVeloY)*/
                    SetCurrentState(States.fallToRoll);
                    //else SetCurrentState(States.hardLanding);
                }
            }
            else if (currentState != States.defInAir) { SetCurrentState(States.defInAir); }

            if (currentState == States.boxJump && (hit.distance > maxBoxJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);
            else if (currentState == States.walkingJump && (hit.distance > maxWalkingJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);
            else if (currentState == States.runningJump && (hit.distance > maxRunningJumpHeight || hit.distance == 0)) SetCurrentState(States.defInAir);


        }

        private void SetCurrentState(States state) { currentState = state; }

        private void CheckForComponents()
        {
            if (!(anim = GetComponent<Animator>()))
            {
                Debug.LogWarning("Missing Animator on " + this.name);
                inhibitMove = true;
            }
            if (anim.avatar)
                if (!anim.avatar.isValid)
                    Debug.LogWarning("Animator avatar is not valid");
        }
    }
}
