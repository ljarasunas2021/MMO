using UnityEngine;
using System.Collections.Generic;

public class AnimatorFollow : MonoBehaviour
{
    [Header("NonRagdoll")]
    [SerializeField] [Tooltip("Animated non ragdoll")] private GameObject nonRagdoll;

    [Header("Limbs")]
    [SerializeField] [Tooltip("Limbs to be animated physically")] private Limb[] limbs;

    [Header("Movements")]
    [Tooltip("Locomotion Animation Settings")] public Anim locomotionAnim;
    [Tooltip("Falling Animation Settings")] public Anim fallingAnim;

    [Header("Other")]
    [SerializeField] [Tooltip("Should the animated non ragdoll be hidden?")] private bool hideNonRagdoll;
    [SerializeField] private bool hideRagdoll;

    [HideInInspector] [Tooltip("Current animation that's being played")] public Anim currentAnim;
    private bool isLocalPlayer;
    private Movement movement;

    private void Awake()
    {
        Anim[] anims = new Anim[] { locomotionAnim, fallingAnim };
        movement = GetComponent<Movement>();

        // set certain variables for each limb
        foreach (Limb limb in limbs)
        {
            limb.posToCOM = Quaternion.Inverse(limb.rigidbody.transform.rotation) * limb.rigidbody.worldCenterOfMass - limb.rigidbody.transform.position;
            limb.configurableJoint = limb.rigidbody.GetComponent<ConfigurableJoint>();
            if (limb.configurableJoint != null)
            {
                limb.localToJointSpace = Quaternion.LookRotation(Vector3.Cross(limb.configurableJoint.axis, limb.configurableJoint.secondaryAxis), limb.configurableJoint.secondaryAxis);
                limb.startLocalRotation = limb.rigidbody.transform.localRotation * limb.localToJointSpace;
            }
            limb.SetUp(anims);
        }
    }

    private void Start()
    {
        ChangeCurrentAnim(locomotionAnim);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        AnimFollow();
    }

    public void AnimFollow()
    {
        Model();

        Follow();
    }

    private void Model()
    {
        float torqueAngle;
        Vector3 torqueSignal, torqueError, torqueAxis;

        foreach (Limb limb in limbs)
        {
            Quaternion targetRotation = limb.limb.transform.rotation * Quaternion.Inverse(limb.rigidbody.rotation);
            targetRotation.ToAngleAxis(out torqueAngle, out torqueAxis);
            torqueError = FixEuler(torqueAngle) * torqueAxis;

            torqueSignal = currentAnim.torquePID.Update(torqueError, currentAnim, limb, PIDType.torque);
            torqueSignal = Vector3.ClampMagnitude(torqueSignal, currentAnim.maxTorque);
            limb.rigidbody.AddTorque(torqueSignal, ForceMode.VelocityChange);

            if (limb.configurableJoint != null) limb.configurableJoint.targetRotation = Quaternion.Inverse(limb.localToJointSpace) * Quaternion.Inverse(limb.limb.transform.localRotation) * limb.startLocalRotation;
        }
    }

    ///<summary> Make the ragdoll follow the non ragdoll by adding forces to the ragdoll until the positions match </summary>
    private void Follow()
    {
        Vector3 forceSignal, forceError;
        foreach (Limb limb in limbs)
        {
            forceError = limb.limb.transform.position + limb.limb.transform.rotation * limb.posToCOM - limb.rigidbody.worldCenterOfMass;
            forceSignal = currentAnim.forcePID.Update(forceError, currentAnim, limb, PIDType.force);
            forceSignal = Vector3.ClampMagnitude(forceSignal, currentAnim.maxForce);
            limb.rigidbody.AddForce(forceSignal, ForceMode.VelocityChange);
        }
    }

    ///<summary> Set the strength of the torque of the configurable joints </summary>
    public void SetJointDrive(float positionSpring, float positionDamper, float maximumForce)
    {
        JointDrive jointDrive = new JointDrive();
        jointDrive.positionSpring = positionSpring;
        jointDrive.positionDamper = positionDamper;
        jointDrive.maximumForce = maximumForce;

        foreach (Limb limb in limbs)
        {
            if (limb.configurableJoint != null) limb.configurableJoint.slerpDrive = jointDrive;
        }
    }

    ///<summary> Enable/disable the limits of the joints </summary>
    public void EnableJointLimits(bool jointLimits)
    {
        ConfigurableJointMotion limitMotion = (jointLimits) ? ConfigurableJointMotion.Limited : ConfigurableJointMotion.Free;

        foreach (Limb limb in limbs)
        {
            if (limb.configurableJoint != null)
            {
                limb.configurableJoint.angularXMotion = limitMotion;
                limb.configurableJoint.angularYMotion = limitMotion;
                limb.configurableJoint.angularZMotion = limitMotion;
            }
        }
    }

    ///<summary> Fix an angle to get it in between 180 and -180 </summary>
    private float FixEuler(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }

    ///<summary> Set the current movement and all of its values </summary>
    public void ChangeCurrentAnim(Anim anim)
    {
        currentAnim = anim;

        SetJointDrive(anim.positionSpring, anim.positionDamper, anim.maximumForce);

        EnableJointLimits(anim.jointLimits);
    }
}

// class used to store information about each limb
[System.Serializable]
public class Limb
{
    [Tooltip("Non ragdoll limb")] public AnimLimb limb;
    [Tooltip("Ragdoll rigidbody")] public Rigidbody rigidbody;
    [HideInInspector] [Tooltip("Vector 3 relating the rigidbody's position to its center of mass")] public Vector3 posToCOM;
    [HideInInspector] [Tooltip("Quaternion that converts from local to joint space")] public Quaternion localToJointSpace;
    [HideInInspector] [Tooltip("Quaternion that stores the starting local rotation")] public Quaternion startLocalRotation;
    [HideInInspector] [Tooltip("Configurable joint of the rigidbody")] public ConfigurableJoint configurableJoint;
    [HideInInspector] [Tooltip("2D dictionary storing data used for the PID controller that contains the integral and previous errors")] public Dictionary<Anim, Dictionary<PIDType, PIDData>> PIDData = new Dictionary<Anim, Dictionary<PIDType, PIDData>>();

    public void SetUp(Anim[] anims)
    {
        foreach (Anim anim in anims)
        {
            PIDProfiles profile = null;
            foreach (RGAndPIDProfiles rGAndPIDProfile in anim.rGAndPIDProfiles)
            {
                if (rGAndPIDProfile.rigidbody == this.rigidbody) profile = rGAndPIDProfile.pIDProfiles;
            }
            Dictionary<PIDType, PIDData> dict = new Dictionary<PIDType, PIDData>();
            dict.Add(PIDType.force, new PIDData(profile, PIDType.force));
            dict.Add(PIDType.torque, new PIDData(profile, PIDType.torque));
            PIDData.Add(anim, dict);
        }
    }
}

// class used to create a PID controller that controls the torque or force adding to a rigidbody
// PID controller stands for proportional, integral, and derivative controller that uses the previous to minimize the amount of error between a current and target value
[System.Serializable]
public class PID
{
    [Tooltip("Weight of proportionality in minimizing the error")] public float pFactor;
    [Tooltip("Weight of the integral in minimizing the error")] public float iFactor;
    [Tooltip("Weight of the derivative in minimizing the error")] public float dFactor;

    ///<summary> Retrun the appropriate signal based on the error and the last error and integral which can be found using the current anim, limb, and type </summary>
    public Vector3 Update(Vector3 error, Anim currentAnim, Limb limb, PIDType type)
    {
        PIDData data = limb.PIDData[currentAnim][type];

        Vector3 integral = data.integral;
        Vector3 lastError = data.lastError;

        integral += error * Time.fixedDeltaTime;
        Vector3 deriv = (error - lastError) / Time.fixedDeltaTime;
        lastError = error;

        data.integral = integral;
        data.lastError = lastError;
        limb.PIDData[currentAnim][type] = data;

        return error * pFactor * data.pIDProfiles[PIDFactors.p] + integral * iFactor * data.pIDProfiles[PIDFactors.i] + deriv * dFactor * data.pIDProfiles[PIDFactors.d];
    }
}

// a movement where certain anim follow variables are defined
[System.Serializable]
public class Anim
{
    [Tooltip("Should this movement be restricted to the joint limits of the ragdoll")] public bool jointLimits;

    [Header("Force PID")]
    [Tooltip("The max force that should be added to the ragdoll")] public float maxForce;
    [Tooltip("The PID variables for the force")] public PID forcePID;

    [Header("Torque PID")]
    [Tooltip("The max torque that should be added to the ragdoll")] public float maxTorque;
    [Tooltip("The PID variables for the torque")] public PID torquePID;

    [Header("Configurable Joint Values")]
    [Tooltip("The spring torque used to rotate the joint")] public float positionSpring;
    [Tooltip("Dampers the amount of spring torque when there are large gaps")] public float positionDamper;
    [Tooltip("Limits the amount of force the joint can apply")] public float maximumForce;

    [Header("Special PID Profiles")]
    [Tooltip("Rigidbodies that have special PIDs")] public RGAndPIDProfiles[] rGAndPIDProfiles;
}

// an enum with the type of PID used
public enum PIDType
{
    force,
    torque
}

// an enum with the factors the pid controller uses (stand for proportionality, integral, and derivative)
public enum PIDFactors
{
    p,
    i,
    d
}

// data stored for use by the PID Controller - limb, anim, and PID type specific
public class PIDData
{
    [Tooltip("The sum of the previous errors")] public Vector3 integral;
    [Tooltip("The previous error")] public Vector3 lastError;
    [Tooltip("Dictionary containing the PID factor and a corresponding weight for the limb")] public Dictionary<PIDFactors, float> pIDProfiles = new Dictionary<PIDFactors, float>();

    public PIDData(PIDProfiles profiles, PIDType type)
    {
        integral = Vector3.zero;
        lastError = Vector3.zero;
        pIDProfiles[PIDFactors.p] = (profiles != null) ? ((type == PIDType.force) ? profiles.pForceProfile : profiles.pTorqueProfile) : 1;
        pIDProfiles[PIDFactors.i] = (profiles != null) ? ((type == PIDType.force) ? profiles.iForceProfile : profiles.iTorqueProfile) : 1;
        pIDProfiles[PIDFactors.d] = (profiles != null) ? ((type == PIDType.force) ? profiles.dForceProfile : profiles.dTorqueProfile) : 1;
    }
}

// rigidbody with profiles stored allowing unique PID values for each body part
[System.Serializable]
public class RGAndPIDProfiles
{
    [Tooltip("Rigidbody that needs special PID values")] public Rigidbody rigidbody;
    [Tooltip("Special PID values")] public PIDProfiles pIDProfiles;
}

// PID profiles to tweak for each body part
[System.Serializable]
public class PIDProfiles
{
    [Tooltip("Proportionality weight of the limb when in the force PID")] public float pForceProfile = 1;
    [Tooltip("Integral weight of the limb when in the force PID")] public float iForceProfile = 1;
    [Tooltip("Derivative weight of the limb when in the force PID")] public float dForceProfile = 1;
    [Tooltip("Proportionality weight of the limb when in the torque PID")] public float pTorqueProfile = 1;
    [Tooltip("Integral weight of the limb when in the torque PID")] public float iTorqueProfile = 1;
    [Tooltip("Derivative weight of the limb when in the torque PID")] public float dTorqueProfile = 1;
}


