using System.Linq;
using UnityEngine;
using Mirror;

namespace MMO.Player
{
    /// <summary> Control player's movement </summary>
    [RequireComponent(typeof(Animator))]
    public class Movement : NetworkBehaviour
    {
        [Header("GameObjects")]
        // player's hips
        [SerializeField] private GameObject hips;

        [Header("Locomotion Blend Values")]
        // blend values for locomotion animations
        [SerializeField] private float idleVal;
        [SerializeField] private float walkVal;
        [SerializeField] private float runVal;

        [Header("Locomotion Blend Value Thresholds")]
        // midpoints for the blend values, so that the appropriate jump animation is played
        [SerializeField] private float idleToWalkThreshold;
        [SerializeField] private float walkToRunThreshold;

        [Header("Smooth Time Values")]
        // time it takes to turn
        [SerializeField] private float turnSmoothTime;
        // time it takes to accelerate
        [SerializeField] private float locomotionAccelerationSmoothTime;
        // time it takes to decelerate
        [SerializeField] private float locomotionDecelerationSmoothTime;

        [Header("Mid Air Values")]
        // player's minimum distance from groud for the player to be "in air"
        [SerializeField] private float minDistFromGroundToBeMidAir;
        // gravity applied to player
        [SerializeField] private float gravity;

        [Header("Landing Velocity Y's")]
        // max time in air for a soft landing to executed
        [SerializeField] private float softLandingMaxTimeInAir;
        // max time in air for a roll landing to executed
        [SerializeField] private float rollLandingMaxTimeInAir;

        [Header("Jump Maximum Distances From Ground")]
        // box jump max height
        [SerializeField] private float maxBoxJumpHeight;
        // walking jump max height
        [SerializeField] private float maxWalkingJumpHeight;
        // running jump max height
        [SerializeField] private float maxRunningJumpHeight;

        [Header("Camera Values")]
        // camera's y rotation offset
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
        // current locmotion vars values
        private float locomotionBlendVal;
        private float locomotionDirectionVal;

        // current locomotion state
        [HideInInspector] public PlayerAnimState currentState;
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

        /// <summary> Init vars </summary>
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

        /// <summary> Move the player </summary>
        public void Move()
        {
            currentState = (PlayerAnimState)animator.GetInteger(PlayerAnimParameters.currentState);

            Vector2 inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = inputVector.normalized;

            MoveWASD();

            SetLocomotionValues(inputVector, Input.GetButton("Sprint"));

            RotatePlayer(inputDir, Input.GetButton("Free Rotate Camera"), Input.mousePosition);

            CheckForJump(inputVector, Input.GetButton("Jump"));

            SetValuesIfMidAir(Input.GetButton("Jump"));

            animator.SetInteger(PlayerAnimParameters.currentState, (int)currentState);
        }

        /// <summary> Move using WASD </summary>
        private void MoveWASD()
        {
            bool lockedCameraMode = playerCameraManager.ReturnCameraMode() == CameraModes.locked;
            Transform transformToUse = (lockedCameraMode) ? Camera.main.transform : transform;
            cc.Move(transformToUse.forward * animator.GetFloat(PlayerAnimParameters.currentSpeedZ) * Time.deltaTime);
            cc.Move(transformToUse.right * animator.GetFloat(PlayerAnimParameters.currentSpeedX) * Time.deltaTime);
        }

        /// <summary> Set the locomotion blend and dir values </summary>
        /// <param name="input"> WASD input as vector 2 </param>
        /// <param name="leftShift"> if the left shift is held down </param>
        private void SetLocomotionValues(Vector2 input, bool leftShift)
        {
            if (currentState != PlayerAnimState.locomotion) return;

            float targetLocomotionBlendVal = 0;
            float targetLocomotionDirection = 0;

            if (UIManager.instance.canMove)
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

            float locomotionDirSmoothTime = (targetLocomotionDirection - locomotionDirectionVal > 0) ? locomotionAccelerationSmoothTime : locomotionDecelerationSmoothTime;
            locomotionDirectionVal = Mathf.SmoothDamp(locomotionDirectionVal, targetLocomotionDirection, ref locomotionDirSmoothVelocity, locomotionDirSmoothTime);

            float locomotionSmoothTime = (targetLocomotionBlendVal - locomotionBlendVal > 0) ? locomotionAccelerationSmoothTime : locomotionDecelerationSmoothTime;
            locomotionBlendVal = Mathf.SmoothDamp(locomotionBlendVal, targetLocomotionBlendVal, ref locomotionSmoothVelocity, locomotionSmoothTime);

            animator.SetFloat(PlayerAnimParameters.locomotionBlend, locomotionBlendVal);
            animator.SetFloat(PlayerAnimParameters.locomotionDir, locomotionDirectionVal);
        }

        /// <summary> Rotate the player </summary>
        /// <param name="inputDir"> WASD input as vector 2 </param>
        /// <param name="leftControl"> if the player holds left control </param>
        /// <param name="mousePos"> the position of the player's mouse </param>
        private void RotatePlayer(Vector2 inputDir, bool leftControl, Vector2 mousePos)
        {
            if (!UIManager.instance.canMove) return;

            if (currentCam != CameraModes.locked)
            {
                if (inputDir != Vector2.zero && animator.GetBool(PlayerAnimParameters.canRotate))
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

        /// <summary> Jump if necessary </summary>
        /// <param name="input"> player's WASD input as vector 2 </param>
        /// <param name="space"> if the player pressed the space bar </param>
        private void CheckForJump(Vector2 input, bool space)
        {
            if (!UIManager.instance.canMove) return;

            if (space)
            {
                if (locomotionBlendVal <= idleToWalkThreshold) SetCurrentState(PlayerAnimState.boxJump);
                else if (locomotionBlendVal >= walkToRunThreshold) SetCurrentState(PlayerAnimState.runningJump);
                else SetCurrentState(PlayerAnimState.walkingJump);
            }
        }

        /// <summary> Deal with movement in air </summary>
        /// <param name="space"> if the player presses the space key </param>
        private void SetValuesIfMidAir(bool space)
        {
            RaycastHit hit;
            Ray ray = new Ray(hips.transform.position, Vector3.down);
            Physics.Raycast(ray, out hit, maxRaycastDownDist, 1 << LayerMaskController.environment);

            if (hit.distance < minDistFromGroundToBeMidAir && hit.distance != 0)
            {
                if (currentState == PlayerAnimState.defInAir)
                {
                    if (timeInAir < softLandingMaxTimeInAir) SetCurrentState(PlayerAnimState.softLanding);
                    else if (timeInAir < rollLandingMaxTimeInAir) SetCurrentState(PlayerAnimState.fallToRoll);
                    else SetCurrentState(PlayerAnimState.hardLanding);
                }
                timeInAir = 0;
            }
            else
            {
                if (currentState != PlayerAnimState.defInAir)
                {
                    SetCurrentState(PlayerAnimState.defInAir);
                }
                timeInAir += Time.deltaTime;
            }

            if (cc.isGrounded) velocityY = 0;
            else velocityY += gravity * Time.deltaTime;

            cc.Move(Vector3.up * velocityY);

            if (cc.isGrounded) velocityY = 0;

            if (currentState == PlayerAnimState.boxJump && (hit.distance > maxBoxJumpHeight || hit.distance == 0)) SetCurrentState(PlayerAnimState.defInAir);
            else if (currentState == PlayerAnimState.walkingJump && (hit.distance > maxWalkingJumpHeight || hit.distance == 0)) SetCurrentState(PlayerAnimState.defInAir);
            else if (currentState == PlayerAnimState.runningJump && (hit.distance > maxRunningJumpHeight || hit.distance == 0)) SetCurrentState(PlayerAnimState.defInAir);
        }

        /// <summary> Set the current state var </summary>
        /// <param name="state"> what the current state should be </param>
        private void SetCurrentState(PlayerAnimState state) { currentState = state; }

        /// <summary> Set the current cam var </summary>
        /// <param name="currentCam"> what the current cam should be </param>
        public void SetCurrentCam(CameraModes currentCam) { this.currentCam = currentCam; }
    }


    ///<summary> Animations and indexes associated with the player's animations </summary>
    public enum PlayerAnimState
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

    ///<summary> Animations and indexes associated with the player's upper body animations </summary>
    public enum PlayerAnimUpperBodyState
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

    /// <summary> Names of parameters used in player's animation controller </summary>
    public static class PlayerAnimParameters
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
}
