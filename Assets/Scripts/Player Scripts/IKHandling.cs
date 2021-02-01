using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Handles player's IK </summary>
public class IKHandling : MonoBehaviour
{
    // how much ik should be applied
    [Range(0, 1)]
    public float ikWeight = 1;
    // distance from ground to center of body
    public float offsetY;
    // look iks to be applied when player is not equipped (basic) / equipped
    public LookIK basicLookIK, equippedLookIK;
    // the current look ik (basic or equipped)
    private LookIK currentLookIK;
    // ground angle under left/right foot
    private float groundAngleLeft, groundAngleRight;
    // transform of the left/right foot
    private Transform leftFoot, rightFoot;
    // animator component of player
    private Animator anim;
    // left foot / right foot / where the player is looking positions
    private Vector3 leftFootPos, rightFootPos, lookPos;
    // left / right foot rotations
    private Quaternion leftFootRot, rightFootRot;
    // the main camera
    private Camera mainCam;

    /// <summary> Init vars </summary>
    void Start()
    {
        anim = GetComponent<Animator>();
        // leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        // rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        // leftFootRot = leftFoot.rotation;
        // rightFootRot = rightFoot.rotation;
        mainCam = Camera.main;
        currentLookIK = basicLookIK;
    }

    /// <summary> Switch the current look IK </summary>
    /// <param name="type"> the type of look IK that should be switched to </param>
    public void SwitchLookIK(LookIKTypes type)
    {
        if (type == LookIKTypes.Basic) currentLookIK = basicLookIK;
        else if (type == LookIKTypes.Weapon) currentLookIK = equippedLookIK;
    }

    /// <summary> Find look pos </summary>
    void Update()
    {
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward);
        lookPos = ray.GetPoint(currentLookIK.lookDistance);

        // RaycastHit leftHit, rightHit;

        // Vector3 leftPos = leftFoot.TransformPoint(Vector3.zero);
        // Vector3 rightPos = rightFoot.TransformPoint(Vector3.zero);

        // if (Physics.Raycast(leftPos, Vector3.down, out leftHit, raycastDistance))
        // {
        //     leftFootPos = leftHit.point;
        //     leftFootRot = Quaternion.FromToRotation(transform.up, leftHit.normal) * transform.rotation;
        //     groundAngleLeft = Vector3.Angle(Vector3.up, leftHit.normal);
        // }

        // if (Physics.Raycast(rightPos, Vector3.down, out rightHit, raycastDistance))
        // {
        //     rightFootPos = rightHit.point;
        //     rightFootRot = Quaternion.FromToRotation(transform.up, rightHit.normal) * transform.rotation;
        //     groundAngleRight = Vector3.Angle(Vector3.up, rightHit.normal);
        // }
    }

    /// <summary> Apply the IK. This function is called automatically by Unity </summary>
    void OnAnimatorIK()
    {
        anim.SetLookAtWeight(currentLookIK.lookIKWeight, currentLookIK.bodyWeight, currentLookIK.headWeight, currentLookIK.eyesWeight, currentLookIK.clampWeight);
        anim.SetLookAtPosition(lookPos);

        // float leftFootWeight = anim.GetFloat(Parameters.leftFoot);
        // float rightFootWeight = anim.GetFloat(Parameters.rightFoot);

        // anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight * ikWeight);
        // anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight * ikWeight);

        // anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos + Vector3.up * offsetY);
        // anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos + Vector3.up * offsetY);

        // anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight * ikRotWeight);
        // anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, leftFootWeight * ikRotWeight);

        // anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRot);
        // anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRot);
    }
}

/// <summary> Class that holds Unity variables for a IK</summary>
[System.Serializable]
public class LookIK
{
    public float lookIKWeight, bodyWeight, headWeight, eyesWeight, clampWeight, lookDistance;
}

/// <summary> Types of look IKs</summary>
public enum LookIKTypes
{
    Basic,
    Weapon
}
