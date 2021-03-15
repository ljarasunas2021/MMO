using UnityEngine;
using System.Collections;

namespace AnimFollow
{
    public class AnimFollow_AF : MonoBehaviour
    {
        [HideInInspector] public RagdollControl_AF ragdollControl;

        [Header("Basic")]
        [SerializeField] [Tooltip("Use World torque to controll the ragdoll")] private bool torque = false;
        [SerializeField] [Tooltip("Use force to controll the ragdoll")] private bool force = true;
        [SerializeField] [Tooltip("Hide the mesh renderers of the master")] private bool hideMaster = true;

        [Header("Root")]
        [Tooltip("")] public GameObject master;
        [SerializeField] [Tooltip("Transforms to exclude from slaves")] private Transform[] slaveExcludeTransforms;

        [Header("Physics Variables")]
        [SerializeField] [Tooltip("If you choose to go to longer times you need to lower PTorque, PLocalTorque and PForce or the system gets unstable. Can be done, longer time is better performance but worse mimicking of master")] private float fixedDeltaTime = 0.01f;
        [Range(0f, 340f)] [Tooltip("Rigidbodies angular drag")] public float angularDrag = 100f;
        [Range(0f, 2f)] [Tooltip("Rigidbodies drag")] public float drag = .5f;

        [Header("Torques / Forces")]
        [Range(0f, 100f)] [Tooltip("Limits the world space torque")] public float maxTorque = 100f;
        [Range(0f, 100f)] [Tooltip("Limits the force")] public float maxForce = 100f;
        [Range(0f, 10000f)] [Tooltip("Limits the force")] public float maxJointTorque = 10000f;
        [Range(0f, 10f)] [SerializeField] [Tooltip("Limits the force")] private float jointDamping = .6f;
        [Tooltip("Individual limits for torque per limb")] public float[] maxTorqueProfile = { 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f, 100f };
        [Tooltip("Individual limits for force per limb")] public float[] maxForceProfile = { 1f, .2f, .2f, .2f, .2f, 1f, 1f, .2f, .2f, .2f, .2f, .2f };
        [Tooltip("Individual limits for joint torque per limb")] public float[] maxJointTorqueProfile = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
        [Tooltip("Multiplier for how much joint's position damper is changed")] public float[] jointDampingProfile = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };
        [Tooltip("Degree to which an error in force matters per limb")] public float[] forceErrorWeightProfile = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // Per limb error weight

        [Header("PD Controller")]
        [Range(0f, .64f)] [SerializeField] [Tooltip("Torque strength for all limbs")] private float PTorque = .16f; // For all limbs Torque strength
        [Range(0f, 160f)] [SerializeField] [Tooltip("Force strength for all limbs")] private float PForce = 30f;
        [Range(0f, .008f)] [SerializeField] [Tooltip("Derivative multiplier for PTorque")] private float DTorque = .002f; // Derivative multiplier to PD controller
        [Range(0f, .064f)] [SerializeField] [Tooltip("Derivative multiplier for PForce")] private float DForce = .01f;
        [Tooltip("Torque strength multiplier for each limb")] public float[] PTorqueProfile = { 20f, 30f, 10f, 30f, 10f, 30f, 30f, 30f, 30f, 10f, 30f, 10f }; // Per limb world space torque strength for EthanRagdoll_12 (twelve rigidbodies)
        [Tooltip("Force strength multiplier for each limb")] public float[] PForceProfile = { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f };

        private Transform[] masterTransforms; // These are all auto assigned
        private Transform[] masterRigidTransforms = new Transform[1];
        private Transform[] slaveTransforms;
        private Rigidbody[] slaveRigidbodies = new Rigidbody[1];
        private Vector3[] rigidbodiesPosToCOM;
        private Transform[] slaveRigidTransforms = new Transform[1];

        private Quaternion[] localRotations1 = new Quaternion[1];
        private Quaternion[] localRotations2 = new Quaternion[1];

        private float maxAngularVelocity = 1000f; // Rigidbodies maxAngularVelocity. Unitys parameter

        private bool mimicNonRigids = true; // Set all local rotations of the transforms without rigidbodies to match the local rotations of the master

        [Range(2, 100)] [HideInInspector] public int secondaryUpdate = 2;
        private int frameCounter;

        private bool userNeedsToAssignStuff = false;

        private Vector3[] torqueLastError = new Vector3[1];
        private Vector3 totalTorqueError; // Total world space angular error of all limbs. This is a vector.

        private Vector3[] forceLastError = new Vector3[1];
        [HideInInspector] public Vector3 totalForceError; // Total world position error. a vector.

        private Quaternion[] startLocalRotation = new Quaternion[1];
        private ConfigurableJoint[] configurableJoints = new ConfigurableJoint[1];
        private Quaternion[] localToJointSpace = new Quaternion[1];
        private JointDrive jointDrive = new JointDrive();

        void Awake() // Initialize
        {
            Time.fixedDeltaTime = fixedDeltaTime; // Set the physics loop update intervall

            if (!master)
            {
                UnityEngine.Debug.LogWarning("master not assigned in AnimFollow script on " + this.name + "\n");
                userNeedsToAssignStuff = true;
                return;
            }

            else if (!master.GetComponent<SimpleFootIK_AF>())
            {
                UnityEngine.Debug.LogWarning("Missing script SimpleFootIK_AF on " + master.name + ".\nAdd it or comment out the directive from top line in the AnimFollow script." + "\n");
                userNeedsToAssignStuff = true;
            }
            else if (hideMaster)
            {
                SkinnedMeshRenderer visible;
                MeshRenderer visible2;
                if (visible = master.GetComponentInChildren<SkinnedMeshRenderer>())
                {
                    visible.enabled = false;
                    SkinnedMeshRenderer[] visibles;
                    visibles = master.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (SkinnedMeshRenderer visiblen in visibles)
                        visiblen.enabled = false;
                }
                if (visible2 = master.GetComponentInChildren<MeshRenderer>())
                {
                    visible2.enabled = false;
                    MeshRenderer[] visibles2;
                    visibles2 = master.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer visiblen2 in visibles2)
                        visiblen2.enabled = false;
                }
            }

            if (!(ragdollControl = GetComponent<RagdollControl_AF>()))
            {
                UnityEngine.Debug.LogWarning("Missing script RagdollControl on " + this.name + ".\nAdd it or comment out the directive from top line in the AnimFollow script." + "\n");
                userNeedsToAssignStuff = true;
            }

            slaveTransforms = GetComponentsInChildren<Transform>(); // Get all transforms in ragdoll. THE NUMBER OF TRANSFORMS MUST BE EQUAL IN RAGDOLL AS IN MASTER!
            masterTransforms = master.GetComponentsInChildren<Transform>(); // Get all transforms in master. 

            System.Array.Resize(ref localRotations1, slaveTransforms.Length);
            System.Array.Resize(ref localRotations2, slaveTransforms.Length);
            System.Array.Resize(ref rigidbodiesPosToCOM, slaveTransforms.Length);

            if (!(masterTransforms.Length == slaveTransforms.Length))
            {
                UnityEngine.Debug.LogWarning(this.name + " does not have a valid master.\nMaster transform count does not equal slave transform count." + "\n");
                userNeedsToAssignStuff = true;
                return;
            }

            slaveRigidbodies = GetComponentsInChildren<Rigidbody>();
            System.Array.Resize(ref masterRigidTransforms, slaveRigidbodies.Length);
            System.Array.Resize(ref slaveRigidTransforms, slaveRigidbodies.Length);

            System.Array.Resize(ref maxTorqueProfile, slaveRigidbodies.Length);
            System.Array.Resize(ref maxForceProfile, slaveRigidbodies.Length);
            System.Array.Resize(ref maxJointTorqueProfile, slaveRigidbodies.Length);
            System.Array.Resize(ref jointDampingProfile, slaveRigidbodies.Length);
            System.Array.Resize(ref PTorqueProfile, slaveRigidbodies.Length);
            System.Array.Resize(ref PForceProfile, slaveRigidbodies.Length);
            System.Array.Resize(ref forceErrorWeightProfile, slaveRigidbodies.Length);

            System.Array.Resize(ref torqueLastError, slaveRigidbodies.Length);
            System.Array.Resize(ref forceLastError, slaveRigidbodies.Length);

            System.Array.Resize(ref startLocalRotation, slaveRigidbodies.Length);
            System.Array.Resize(ref configurableJoints, slaveRigidbodies.Length);
            System.Array.Resize(ref localToJointSpace, slaveRigidbodies.Length);

            int slaveTFIndex = 0;
            int slaveRGIndex = 0;
            int slaveCJIndex = 0;
            int slaveTF2Index = 0;
            foreach (Transform slaveTransform in slaveTransforms) // Sort the transform arrays
            {
                if (slaveTransform.GetComponent<Rigidbody>())
                {
                    slaveRigidTransforms[slaveRGIndex] = slaveTransform;
                    masterRigidTransforms[slaveRGIndex] = masterTransforms[slaveTFIndex];
                    if (slaveTransform.GetComponent<ConfigurableJoint>())
                    {
                        configurableJoints[slaveRGIndex] = slaveTransform.GetComponent<ConfigurableJoint>();
                        localToJointSpace[slaveRGIndex] = Quaternion.LookRotation(Vector3.Cross(configurableJoints[slaveRGIndex].axis, configurableJoints[slaveRGIndex].secondaryAxis), configurableJoints[slaveRGIndex].secondaryAxis);
                        startLocalRotation[slaveRGIndex] = slaveTransform.localRotation * localToJointSpace[slaveRGIndex];
                        jointDrive = configurableJoints[slaveRGIndex].slerpDrive;
                        jointDrive.mode = JointDriveMode.Position;
                        configurableJoints[slaveRGIndex].slerpDrive = jointDrive;
                        slaveCJIndex++;
                    }
                    else if (slaveRGIndex > 0)
                    {
                        UnityEngine.Debug.LogWarning("Rigidbody " + slaveTransform.name + " on " + this.name + " is not connected to a configurable joint" + "\n");
                        userNeedsToAssignStuff = true;
                        return;
                    }
                    rigidbodiesPosToCOM[slaveRGIndex] = Quaternion.Inverse(slaveTransform.rotation) * (slaveTransform.GetComponent<Rigidbody>().worldCenterOfMass - slaveTransform.position);
                    slaveRGIndex++;
                }
                else
                {
                    bool excludeBool = false;
                    foreach (Transform exclude in slaveExcludeTransforms)
                    {
                        if (slaveTransform == exclude)
                        {
                            excludeBool = true;
                            break;
                        }
                    }

                    if (!excludeBool)
                    {
                        slaveTransforms[slaveTF2Index] = slaveTransform;
                        masterTransforms[slaveTF2Index] = masterTransforms[slaveTFIndex];
                        localRotations1[slaveTF2Index] = slaveTransform.localRotation;
                        slaveTF2Index++;
                    }
                }
                slaveTFIndex++;
            }

            localRotations2 = localRotations1;
            System.Array.Resize(ref masterTransforms, slaveTF2Index);
            System.Array.Resize(ref slaveTransforms, slaveTF2Index);
            System.Array.Resize(ref localRotations1, slaveTF2Index);
            System.Array.Resize(ref localRotations2, slaveTF2Index);

            if (slaveCJIndex == 0)
            {
                UnityEngine.Debug.LogWarning("There are no configurable joints on the ragdoll " + this.name + "\nDrag and drop the ReplaceJoints script on the ragdoll." + "\n");
                userNeedsToAssignStuff = true;
                return;
            }
            else
            {
                SetJointTorque(maxJointTorque);
                EnableJointLimits(false);
            }

            if (slaveRigidTransforms.Length == 0)
                UnityEngine.Debug.LogWarning("There are no rigid body components on the ragdoll " + this.name + "\n");
            else if (slaveRigidTransforms.Length < 12)
                UnityEngine.Debug.Log("This version of AnimFollow works better with one extra colleder in the spine on " + this.name + "\n");

            if (PTorqueProfile[PTorqueProfile.Length - 1] == 0f)
                UnityEngine.Debug.Log("The last entry in the PTorqueProfile is zero on " + this.name + ".\nIs that intentional?\nDrop ResizeProfiles on the ragdoll and adjust the values." + "\n");

            if (slaveExcludeTransforms.Length == 0)
            {
                UnityEngine.Debug.Log("Should you not assign some slaveExcludeTransforms to the AnimFollow script on " + this.name + "\n");
            }
        }

        void Start()
        {
            if (userNeedsToAssignStuff) return;

            foreach (Transform slaveRigidTransform in slaveRigidTransforms) // Set some of the Unity parameters
            {
                Rigidbody slaveRG = slaveRigidTransform.GetComponent<Rigidbody>();
                slaveRG.angularDrag = angularDrag;
                slaveRG.drag = drag;
                slaveRG.maxAngularVelocity = maxAngularVelocity;
                Debug.Log(slaveRigidTransform.gameObject.name);
            }
        }

        void FixedUpdate()
        {
            DoAnimFollow();
        }

        public void DoAnimFollow()
        {
            if (userNeedsToAssignStuff)
                return;

            ragdollControl.DoRagdollControl();

            totalTorqueError = Vector3.zero;
            totalForceError = Vector3.zero;

            if (frameCounter % secondaryUpdate == 0)
            {
                for (int i = 2; i < slaveTransforms.Length - 1; i++) localRotations2[i] = masterTransforms[i].localRotation; // Set all local rotations of the transforms without rigidbodies to match the local rotations of the master
                SetJointTorque(maxJointTorque, jointDamping);
            }

            if (frameCounter % 2 == 0)
            {
                for (int i = 2; i < slaveTransforms.Length - 1; i++) // Set all local rotations of the transforms without rigidbodies to match the local rotations of the master
                {
                    if (secondaryUpdate > 2)
                    {
                        localRotations1[i] = Quaternion.Lerp(localRotations1[i], localRotations2[i], 2f / secondaryUpdate);
                        slaveTransforms[i].localRotation = localRotations1[i];
                    }
                    else slaveTransforms[i].localRotation = localRotations2[i];
                }
            }

            float torqueAngle;
            Vector3 torqueSignal, torqueError, torqueAxis;

            for (int i = 0; i < slaveRigidTransforms.Length; i++) // Do for all rigid bodies
            {
                slaveRigidbodies[i].angularDrag = angularDrag; // Set rigidbody drag and angular drag in real-time
                slaveRigidbodies[i].drag = drag;

                Quaternion targetRotation;
                if (torque) // Calculate and apply world torque
                {
                    targetRotation = masterRigidTransforms[i].rotation * Quaternion.Inverse(slaveRigidTransforms[i].rotation);
                    targetRotation.ToAngleAxis(out torqueAngle, out torqueAxis);
                    torqueError = FixEuler(torqueAngle) * torqueAxis;

                    if (torqueAngle != 360f)
                    {
                        totalTorqueError += torqueError;
                        PDControl(PTorque * PTorqueProfile[i], DTorque, out torqueSignal, torqueError, ref torqueLastError[i], 1 / fixedDeltaTime);
                    }
                    else torqueSignal = new Vector3(0f, 0f, 0f);

                    torqueSignal = Vector3.ClampMagnitude(torqueSignal, maxTorque * maxTorqueProfile[i]);
                    slaveRigidbodies[i].AddTorque(torqueSignal, ForceMode.VelocityChange); // Add torque to the limbs
                }

                // Force error
                Vector3 masterRigidTransformsWCOM = masterRigidTransforms[i].position + masterRigidTransforms[i].rotation * rigidbodiesPosToCOM[i];
                Vector3 forceError = masterRigidTransformsWCOM - slaveRigidTransforms[i].GetComponent<Rigidbody>().worldCenterOfMass; // Doesn't work if collider is trigger
                totalForceError += forceError * forceErrorWeightProfile[i];

                Vector3 forceSignal;

                if (force) // Calculate and apply world force
                {
                    PDControl(PForce * PForceProfile[i], DForce, out forceSignal, forceError, ref forceLastError[i], 1 / fixedDeltaTime);
                    forceSignal = Vector3.ClampMagnitude(forceSignal, maxForce * maxForceProfile[i]);
                    slaveRigidbodies[i].AddForce(forceSignal, ForceMode.VelocityChange);
                }

                if (i > 0) configurableJoints[i].targetRotation = Quaternion.Inverse(localToJointSpace[i]) * Quaternion.Inverse(masterRigidTransforms[i].localRotation) * startLocalRotation[i];
            }
            frameCounter++;
        }

        public void SetJointTorque(float positionSpring, float positionDamper)
        {
            for (int i = 1; i < configurableJoints.Length; i++) // Do for all configurable joints
            {
                jointDrive.positionSpring = positionSpring * maxJointTorqueProfile[i];
                jointDrive.positionDamper = positionDamper * jointDampingProfile[i];
                configurableJoints[i].slerpDrive = jointDrive;
            }
            maxJointTorque = positionSpring;
            jointDamping = positionDamper;
        }

        public void SetJointTorque(float positionSpring)
        {
            for (int i = 1; i < configurableJoints.Length; i++) // Do for all configurable joints
            {
                jointDrive.positionSpring = positionSpring * maxJointTorqueProfile[i];
                configurableJoints[i].slerpDrive = jointDrive;
            }
            maxJointTorque = positionSpring;
        }

        public void EnableJointLimits(bool jointLimits)
        {
            for (int i = 1; i < configurableJoints.Length; i++) // Do for all configurable joints
            {
                if (jointLimits)
                {
                    configurableJoints[i].angularXMotion = ConfigurableJointMotion.Limited;
                    configurableJoints[i].angularYMotion = ConfigurableJointMotion.Limited;
                    configurableJoints[i].angularZMotion = ConfigurableJointMotion.Limited;
                }
                else
                {
                    configurableJoints[i].angularXMotion = ConfigurableJointMotion.Free;
                    configurableJoints[i].angularYMotion = ConfigurableJointMotion.Free;
                    configurableJoints[i].angularZMotion = ConfigurableJointMotion.Free;
                }
            }
        }

        private float FixEuler(float angle) // For the angle in angleAxis, to make the error a scalar
        {
            if (angle > 180f)
                return angle - 360f;
            else
                return angle;
        }

        public static void PDControl(float P, float D, out Vector3 signal, Vector3 error, ref Vector3 lastError, float reciDeltaTime) // A PD controller
        {
            signal = P * (error + D * (error - lastError) * reciDeltaTime);
            lastError = error;
        }
    }
}