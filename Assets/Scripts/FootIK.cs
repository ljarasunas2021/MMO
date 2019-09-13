using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    public float distanceToGround, raycastStartHeight = 1;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        for (int i = 0; i < 2; i++)
        {
            AvatarIKGoal foot = (i == 0) ? AvatarIKGoal.LeftFoot : AvatarIKGoal.RightFoot;
            anim.SetIKPositionWeight(foot, 1);
            anim.SetIKRotationWeight(foot, 1);

            RaycastHit hit;
            Ray ray = new Ray(anim.GetIKPosition(foot) + Vector3.up * raycastStartHeight, Vector3.down);
            if (Physics.Raycast(ray, out hit, distanceToGround + raycastStartHeight, 9))
            {
                Debug.Log(hit.collider.name);
                Vector3 hitPos = hit.point + new Vector3(0, distanceToGround, 0);
                anim.SetIKPosition(foot, hitPos);
                anim.SetIKRotation(foot, Quaternion.LookRotation(transform.forward, hit.normal));
            }
        }
    }
}
