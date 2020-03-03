/*using System.Collections;
using UnityEngine;

namespace AnimFollow
{
    public class RagdollControl_AF : MonoBehaviour
    {
        private Transform ragdollRootBone; // A transform representative of the ragdoll position and rotation. ASSIGN IN INSPECTOR or it will be auto assigned to the first transform with a rigid body

        public GameObject nonRagdollHips;

        [Header("Jump")]
        [SerializeField] private float jumpDrag, jumpAngularDrag;

        [Header("Determines Fall")]
        [SerializeField] [Tooltip("The relative speed limit for a collision to make the character dose off")] private float graceSpeed = 8f;
        [SerializeField] [Tooltip("The Limit of limbError that is allowed before the character doses off, given certain conditions")] private float maxOrientatedError = .5f;
        [SerializeField] [Tooltip("The Limit of limbError that is allowed before the character doses off")] private float maxError = .5f;

        [Header("Fall")]
        [SerializeField] [Tooltip("Determines how fast the character loses control after colliding")] private float fallLerp = 1.5f;
        [SerializeField] [Tooltip("The torque immediately after collision")] private float residualTorque = 0f;
        [SerializeField] [Tooltip("The force immediately after collision")] private float residualForce = .1f;
        [SerializeField] [Tooltip("The joint torque immediately after collision")] private float residualJointTorque = 120f;
        [SerializeField] [Tooltip("Custom drag values during fall")] private float fallDrag = 20f;
        [SerializeField] [Tooltip("Custom angular drag values during fall")] private float fallAngularDrag = 20f;

        [Header("Get Up")]
        [SerializeField] [Tooltip("These rigidbodies will get slippery during getup to avoid snagging")] private Transform[] IceOnGetup;
        [SerializeField] [Tooltip("Custom drag values during getup animations")] private float getupAngularDrag = 50f;
        [SerializeField] [Tooltip("Custom drag values during getup animations")] private float getupDrag = 25f;
        [SerializeField] [Tooltip("When ragdollRootBone goes below this speed the falling state is through and the get up starts")] private float settledSpeed = .2f;
        [SerializeField] [Tooltip("Determines the initial regaining of strength after the character fallen to ease the ragdoll to the masters pose")] private float getupLerp1 = .15f;
        [SerializeField] [Tooltip("Determines the regaining of strength during the later part of the get up state")] private float getupLerp2 = 2f;
        [SerializeField] [Tooltip("A number that defines the degree of strength the ragdoll must reach before it is assumed to match the master pose and start the later part of the get up state")] private float wakeUpStrength = .2f;

        [Header("Contact")]
        [SerializeField] [Tooltip("The torque when in contact with other colliders")] private float contactTorque = 1f;
        [SerializeField] [Tooltip("The force when in contact with other colliders")] private float contactForce = 2f;
        [SerializeField] [Tooltip("The joint torque when in contact with other colliders")] private float contactJointTorque = 1000f;
        [SerializeField] [Tooltip("Determines how fast the character loses strength when in contact")] private float toContactLerp = 70f;
        [SerializeField] [Tooltip("Determines how fast the character gains strength after freed from contact")] private float fromContactLerp = 1f;

        [Header("Torques / Forces")]
        [SerializeField] [Tooltip("The torque when not in contact with other colliders")] private float maxTorque = 100f;
        [SerializeField] [Tooltip("The force when not in contact with other colliders")] private float maxForce = 100f;
        [SerializeField] [Tooltip("The joint torque when not in contact with other colliders")] private float maxJointTorque = 10000f;
        [SerializeField] [Tooltip("The limit of error acceptable to consider the ragdoll to be matching the master. Is condition for going to normal operation after getting up")] private float maxErrorWhenMatching = .1f;

        private AnimFollow_AF animFollow; // The script that controlls the muscles of the ragdoll
        private Movement playerMovement; // To tell the character controller to no move when we are dosed off after a collision.
        private Animator animator; // Reference to the animator component.
        private SimpleFootIK_AF simpleFootIK;
        private GameObject master; // The master character that is originally controlled by animations. Auto assigned
        private Transform masterRootBone; // A transform representative of the ragdoll position and rotation. Auto assigned
        [HideInInspector] public bool falling = false; // Is in falling state
        [HideInInspector] public bool gettingUp = false; // Is in getUp state
        [HideInInspector] public bool jointLimits = false;
        [HideInInspector] public bool jumping = false;

        private float orientateY = 0f; // The world y-coordinate the master transform will be at after a fall. If you move your character vertically you want to set this to match
        [HideInInspector] public float collisionSpeed; // The relative speed of the colliding collider
        [HideInInspector] public int numberOfCollisions; // Number of colliders currently in contact with the ragdoll
        private int secondaryUpdateSet; // Read from the AnimFollow script
        private float[] noIceDynFriction; // To save user settings of friction
        private float[] noIceStatFriction;
        private float drag; // Read from the AnimFollow script
        private float angularDrag;

        // These parameters are not for tuning
        private Quaternion rootboneToForward; // Rotation of ragdollRootBone relative to transform.forward
        private bool userNeedsToAssignStuff = false; // If this is true then ....
        private bool delayedGetupDone = false; // Used to delay setting gettingUp to false if still in contakt after get up state
        private bool getupState = false; // Not used in this version
        private Rigidbody rootBoneRG;
        private float raycastDownDist;

        void Awake() // Initialize
        {
            if (!WeHaveAllTheStuff()) // Check
            {
                userNeedsToAssignStuff = true;
                return;
            }

            secondaryUpdateSet = animFollow.secondaryUpdate;

            SetAnimFollowMaxForces(maxTorque, maxForce, maxJointTorque); // Set the maxTorque in the AnimFollow script. This overrides the settings in AnimFollow

            Rigidbody[] slaveRigidBodies = GetComponentsInChildren<Rigidbody>(); // Get all rigid bodies
            foreach (Rigidbody slaveRigidBody in slaveRigidBodies) slaveRigidBody.gameObject.AddComponent<Limb_AF>(); // Destribute a collision scripts to all limbs. This script reports to RagdollControll if any limb is in contact with another collider

            System.Array.Resize(ref noIceDynFriction, IceOnGetup.Length);
            System.Array.Resize(ref noIceStatFriction, IceOnGetup.Length);

            for (int m = 0; m < IceOnGetup.Length; m++)
            {
                noIceDynFriction[m] = IceOnGetup[m].GetComponent<Collider>().material.dynamicFriction;
                noIceStatFriction[m] = IceOnGetup[m].GetComponent<Collider>().material.staticFriction;
            }

            drag = animFollow.drag;
            angularDrag = animFollow.angularDrag;

            rootBoneRG = ragdollRootBone.GetComponent<Rigidbody>();

            rootboneToForward = Quaternion.Inverse(masterRootBone.rotation) * master.transform.rotation; // Relative orientation of ragdollRootBone to ragdoll transform

            raycastDownDist = playerMovement.minDistFromGroundToBeMidAir;
        }

        public void DoRagdollControl() // Needs to be synced with AnimFollow
        {
            if (userNeedsToAssignStuff) return;

            // Check if we are in getup state or in transition to getup state
            getupState = playerMovement.currentState == States.getUpFront || playerMovement.currentState == States.getUpFrontMirror || playerMovement.currentState == States.getUpBack || playerMovement.currentState == States.getUpBackMirror;
            bool landingState = playerMovement.currentState == States.hardLanding || playerMovement.currentState == States.fallToRoll || playerMovement.currentState == States.softLanding;

            Vector3 limbError = animFollow.totalForceError; // Get Ragdoll distortion from AnimFollow

            // The code below first checks if we are hit with enough force to fall and then do:
            // inhibit movements in PlayerMovement script, falling, orientate master. ease ragdoll to master pose, play getup animation, go to full strength and anable movements again. 

            RaycastHit hit;

            if (!Physics.Raycast(ragdollRootBone.position, Vector3.down, out hit, raycastDownDist, 1 << LayerMaskController.environment) && !falling && !gettingUp)
            {
                SetAnimFollowDrags(jumpDrag, jumpAngularDrag);
                SetJump(true);
                SetAnimFollowMaxForces(10000, 1, 10000);
                animFollow.PTorque = 100;
                //animFollow.
            }
            else if (numberOfCollisions > 0 && !landingState && (collisionSpeed > graceSpeed || (limbError.magnitude > maxOrientatedError && !(gettingUp || falling || jumping))))
            {
                // // The initial strength immediately after the impact
                // SetAnimFollowMaxForces(residualTorque, residualForce, residualJointTorque);
                // animFollow.SetJointTorque(residualJointTorque); // Do not wait for animfollow.secondaryUpdate

                // animFollow.EnableJointLimits(true);
                // jointLimits = true;
                // animFollow.secondaryUpdate = 100;

                // for (int m = 0; m < IceOnGetup.Length; m++) // turn of iceOnGetup
                // {
                //     IceOnGetup[m].GetComponent<Collider>().material.dynamicFriction = noIceDynFriction[m];
                //     IceOnGetup[m].GetComponent<Collider>().material.staticFriction = noIceStatFriction[m];
                // }

                // SetAnimFollowDrags(fallDrag, fallAngularDrag);

                // SetJump(false);
                // falling = true;
                // gettingUp = false;
                // delayedGetupDone = false;

                // animator.SetInteger(Parameters.currentState, (int)States.knockedOut);

                // if (collisionSpeed > graceSpeed) Debug.Log("Fell on Collision Speed");
                // else Debug.Log("Fell on Limb Error");
            }
            else if (gettingUp) // Code do not run in normal operation
            {
                SetJump(false);

                if (animFollow.maxTorque < wakeUpStrength) // Ease the ragdoll to the master pose. WakeUpStrength limit should be set so that the radoll just has reached the master pose
                {
                    master.transform.Translate((ragdollRootBone.position - masterRootBone.position), Space.World);

                    SetAnimFollowMaxForces(Mathf.Lerp(animFollow.maxTorque, contactTorque, getupLerp1 * Time.fixedDeltaTime), Mathf.Lerp(animFollow.maxForce, contactForce, getupLerp1 * Time.fixedDeltaTime), Mathf.Lerp(animFollow.maxJointTorque, contactJointTorque, getupLerp1 * Time.fixedDeltaTime));

                    animFollow.secondaryUpdate = 20;
                }
                else if (!getupState) // Getting up is done. We are back in Idle (if not delayed)
                {
                    playerMovement.inhibitMove = false; // Master is able to move again

                    simpleFootIK.extraYLerp = 1f;

                    SetAnimFollowDrags(drag, angularDrag);
                    animFollow.secondaryUpdate = secondaryUpdateSet;

                    for (int m = 0; m < IceOnGetup.Length; m++) // turn of iceOnGetup
                    {
                        IceOnGetup[m].GetComponent<Collider>().material.dynamicFriction = noIceDynFriction[m];
                        IceOnGetup[m].GetComponent<Collider>().material.staticFriction = noIceStatFriction[m];
                    }

                    if (limbError.magnitude < maxErrorWhenMatching) // Do not go to full strength unless ragdoll is matching master (delay)
                    {
                        gettingUp = false; // Getting up is done
                        delayedGetupDone = false;
                        playerMovement.inhibitRun = false;
                    }
                }
                else // Lerp the ragdoll to contact strength during get up
                {
                    SetAnimFollowMaxForces(Mathf.Lerp(animFollow.maxTorque, contactTorque, getupLerp2 * Time.fixedDeltaTime), Mathf.Lerp(animFollow.maxForce, contactForce, getupLerp2 * Time.fixedDeltaTime), Mathf.Lerp(animFollow.maxJointTorque, contactJointTorque, getupLerp2 * Time.fixedDeltaTime));
                    animFollow.secondaryUpdate = secondaryUpdateSet * 2;

                    if (jointLimits)
                    {
                        animFollow.EnableJointLimits(false);
                        jointLimits = false;
                    }
                }
            }
            else if (falling)
            {
                SetJump(false);

                animator.SetBool(Parameters.knockedOut, true);

                // Lerp force to zero from residual values
                SetAnimFollowMaxForces(Mathf.Lerp(animFollow.maxTorque, 0f, fallLerp * Time.fixedDeltaTime), Mathf.Lerp(animFollow.maxForce, 0f, fallLerp * Time.fixedDeltaTime), Mathf.Lerp(animFollow.maxJointTorque, 0f, fallLerp * Time.fixedDeltaTime));
                animFollow.SetJointTorque(animFollow.maxJointTorque); // Do not wait for animfollow.secondaryUpdate

                // Orientate master to ragdoll and start transition to getUp when settled on the ground. Falling is over, getting up commences
                if (rootBoneRG.velocity.magnitude < settledSpeed)
                {
                    gettingUp = true;
                    Orientate();
                    playerMovement.inhibitMove = true;
                    animFollow.maxTorque = 0f; // These strengths shold be zero to avoid twitching during orientation
                    animFollow.maxForce = 0f;
                    animFollow.maxJointTorque = 0f;
                    //animator.SetFloat(hash.speedFloat, 0f, 0f, Time.fixedDeltaTime);

                    Vector3 rootBoneForward = ragdollRootBone.rotation * rootboneToForward * Vector3.forward;
                    if (Vector3.Dot(rootBoneForward, Vector3.down) >= 0f) // Check if ragdoll is lying on its back or front, then transition to getup animation
                    {
                        if (playerMovement.currentState == States.getUpFront) animator.SetInteger(Parameters.currentState, (int)States.getUpFront);
                        else animator.SetInteger(Parameters.currentState, (int)States.getUpFrontMirror);
                    }
                    else
                    {
                        if (playerMovement.currentState == States.getUpBack) animator.SetInteger(Parameters.currentState, (int)States.getUpBack);
                        else animator.SetInteger(Parameters.currentState, (int)States.getUpBackMirror);
                    }
                }
            }
            else
            {
                SetAnimFollowDrags(drag, angularDrag);
                SetJump(false);
            }

            collisionSpeed = 0f; // Reset to zero

            // The code below is run also in normal operation (not falling or getting up)

            // Check if we are in contact with other colliders
            if (numberOfCollisions == 0) // Not in contact
            {
                // When not in contact character has maxStrength strength
                if (!(gettingUp || falling) || delayedGetupDone) SetAnimFollowMaxForces(Mathf.Lerp(animFollow.maxTorque, maxTorque, fromContactLerp * Time.fixedDeltaTime), Mathf.Lerp(animFollow.maxForce, maxForce, fromContactLerp * Time.fixedDeltaTime), Mathf.Lerp(animFollow.maxJointTorque, maxJointTorque, fromContactLerp * Time.fixedDeltaTime));
            }
            else // In contact
            {
                // When in contact character has only contact strength
                if (!(gettingUp || falling) || delayedGetupDone) SetAnimFollowMaxForces(Mathf.Lerp(animFollow.maxTorque, contactTorque, toContactLerp * Time.fixedDeltaTime), Mathf.Lerp(animFollow.maxForce, contactForce, toContactLerp * Time.fixedDeltaTime), Mathf.Lerp(animFollow.maxJointTorque, contactJointTorque, toContactLerp * Time.fixedDeltaTime));
            }
        }

        private void Orientate()
        {
            Debug.Log("ORIENTATE");
            falling = false;

            // Here the master gets reorientated to the ragdoll which could have ended its fall in any direction and position
            master.transform.rotation = ragdollRootBone.rotation * Quaternion.Inverse(masterRootBone.rotation) * master.transform.rotation;
            master.transform.rotation = Quaternion.LookRotation(new Vector3(master.transform.forward.x, 0f, master.transform.forward.z), Vector3.up);
            master.transform.Translate(ragdollRootBone.position - masterRootBone.position, Space.World);

            simpleFootIK.extraYLerp = .02f;
            simpleFootIK.leftFootPosition = ragdollRootBone.position + Vector3.up;
            simpleFootIK.rightFootPosition = ragdollRootBone.position + Vector3.up;

            for (int m = 0; m < IceOnGetup.Length; m++) // Turn On iceOnGetup
            {
                IceOnGetup[m].GetComponent<Collider>().material.dynamicFriction = 0f;
                IceOnGetup[m].GetComponent<Collider>().material.staticFriction = 0f;
            }

            SetAnimFollowDrags(getupDrag, getupAngularDrag);
        }

        private void SetJump(bool jump)
        {
            if (jump != jumping)
            {
                /*Debug.Log("Jump " + jump + " jumping " + jumping);

                if (!jump)
                {
                    nonRagdollHips.transform.position = ragdollRootBone.transform.position;
                    nonRagdollHips.transform.rotation = ragdollRootBone.transform.rotation;
                }
                jumping = jump;
            }
        }

        private void SetAnimFollowMaxForces(float maxTorque, float maxForce, float maxJointTorque)
{
    animFollow.maxTorque = maxTorque;
    animFollow.maxForce = maxForce;
    animFollow.maxJointTorque = maxJointTorque;
}

private void SetAnimFollowDrags(float drag, float angularDrag)
{
    animFollow.drag = drag;
    animFollow.angularDrag = angularDrag;
}

bool WeHaveAllTheStuff()
{
    if (!(animFollow = GetComponent<AnimFollow_AF>()))
    {
        Debug.LogWarning("Missing Script: AnimFollow on " + this.name + "\n");
        return (false);
    }
    else if (!(master = animFollow.master))
    {
        Debug.LogWarning("master not assigned in AnimFollow script on " + this.name + "\n");
        return (false);
    }
    else if (!(simpleFootIK = master.GetComponent<SimpleFootIK_AF>()))
    {
        UnityEngine.Debug.LogWarning("Missing script SimpleFootIK script on " + master.name + ".\nAdd it or comment out the directive from top line in the AnimFollow script." + "\n");
        return false;
    }
    else if (!master.activeInHierarchy)
    {
        Debug.LogWarning("Master of " + this.name + " is not active" + "\n");
        return false;
    }
    else
    {
        if (!ragdollRootBone)
        {
            ragdollRootBone = GetComponentInChildren<Rigidbody>().transform;
            //				Debug.Log("ragdollRootBone not assigned in RagdollControll script on " + this.name + ".\nAuto assigning to " + ragdollRootBone.name + "\nThis is probably correct if this is a standard biped." + "\n");
        }
        else if (!ragdollRootBone.GetComponent<Rigidbody>() || !(ragdollRootBone.root == this.transform.root))
        {
            ragdollRootBone = GetComponentInChildren<Rigidbody>().transform;
            Debug.LogWarning("ragdollRootBone in RagdollControll script on " + this.name + " has no rigid body component or is not child of ragdoll.\nAuto assigning to " + ragdollRootBone.name + "\nAuto assignment is probably correct if this is a standard biped." + "\n");
        }
        int i = 0;
        Transform[] transforms = GetComponentsInChildren<Transform>();
        foreach (Transform transformen in transforms) // Find the masterRootBoone
        {
            if (transformen == ragdollRootBone)
            {
                masterRootBone = master.GetComponentsInChildren<Transform>()[i];
                break;
            }
            i++;
        }
    }

    if (!(playerMovement = master.GetComponent<Movement>()))
    {
        Debug.LogWarning("Missing Script: PlayerMovement on " + master.name + "\n");
        return (false);
    }
    if (!(animator = master.GetComponent<Animator>()))
    {
        Debug.LogWarning("Missing Animator on " + master.name + "\n");
        return (false);
    }
    else
    {
        if (animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
            Debug.Log("Animator cullingmode on " + this.name + " is not set to always animate.\nIf the masteris hidden the animations will not run." + "\n");
        if (!animator.updateMode.Equals(AnimatorUpdateMode.AnimatePhysics))
            Debug.Log("Animator on " + this.name + " is not set to animate physics" + "\n");
    }

    if (IceOnGetup.Length == 0)
    {
        Debug.Log("Assign left and right calf and thigh to iceOnGetup in script RagdollControl on " + this.name + "\n");
    }
    else if (IceOnGetup[IceOnGetup.Length - 1] == null)
    {
        Debug.LogWarning("Assign left and right calf and thigh to iceOnGetup in script RagdollControl on " + this.name + "\nDo not leave elements as null." + "\n");
        return false;
    }

    return (true);
}
    }
}
*/
