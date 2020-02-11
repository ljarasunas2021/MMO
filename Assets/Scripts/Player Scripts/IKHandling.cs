using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandling : MonoBehaviour
{
    public float ikWeight = 1, ikRotWeight, ikRotAnglePower, ikRotPower, raycastDistance = 1, offsetY;
    public LookIK basicLookIK, equippedLookIK;
    private LookIK currentLookIK;
    private float groundAngleLeft, groundAngleRight;
    private Transform leftFoot, rightFoot;
    private Animator anim;
    private Vector3 leftFootPos, rightFootPos, lookPos;
    private Quaternion leftFootRot, rightFootRot;
    private Camera mainCam;

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

    public void SwitchLookIK(LookIKTypes type)
    {
        if (type == LookIKTypes.Basic) currentLookIK = basicLookIK;
        else if (type == LookIKTypes.Weapon) currentLookIK = equippedLookIK;
    }

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

[System.Serializable]
public class LookIK
{
    public float lookIKWeight, bodyWeight, headWeight, eyesWeight, clampWeight, lookDistance;
}

public enum LookIKTypes
{
    Basic,
    Weapon
}
