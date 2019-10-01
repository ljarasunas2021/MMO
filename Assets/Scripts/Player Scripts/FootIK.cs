using UnityEngine;

// IGNORE SCRIPT - WILL BE WORKED OUT IN FUTURE AND HAS NO EFFECT ON GAME CURRENTLY
// WILL BE USED TO MAKE SURE LEGS ARE NOT FLOATING OFF GROUND AND HAVE SAME ROTATION AS GROUND

public class FootIK : MonoBehaviour
{
    public float feetToIKPositionSpeed, pelvisVerticalSpeed, raycastDownDistance, heightToStartRaycast, ankleHeight, pelvisHeight;
    public LayerMask environment;
    private Animator anim;
    private Vector3 leftFootPosition, rightFootPosition, leftFootIKPosition, rightFootIKPosition;
    private Quaternion rightFootIKRotation, leftFootIKRotation;
    private float lastRightFootPositionY, lastLeftFootPositionY, lastPelvisPositionY;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        AdjustFeetTarget(ref rightFootPosition, HumanBodyBones.RightFoot);
        AdjustFeetTarget(ref leftFootPosition, HumanBodyBones.LeftFoot);

        FeetPositionSolver(rightFootPosition, ref rightFootIKPosition, ref rightFootIKRotation);
        FeetPositionSolver(leftFootPosition, ref leftFootIKPosition, ref leftFootIKRotation);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        //MovePelvisHeight();

        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        MoveFeetToIKPoint(AvatarIKGoal.RightFoot, rightFootIKPosition, rightFootIKRotation, ref lastRightFootPositionY);

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

        MoveFeetToIKPoint(AvatarIKGoal.LeftFoot, leftFootIKPosition, leftFootIKRotation, ref lastLeftFootPositionY);
    }

    private void MoveFeetToIKPoint(AvatarIKGoal foot, Vector3 positionIKHolder, Quaternion rotationIKHolder, ref float lastFootPositionY)
    {
        Vector3 currentIKPosition = anim.GetIKPosition(foot);

        if (positionIKHolder != Vector3.zero)
        {
            currentIKPosition = transform.InverseTransformPoint(currentIKPosition);
            positionIKHolder = transform.InverseTransformPoint(positionIKHolder);

            float yLerp = Mathf.Lerp(currentIKPosition.y, positionIKHolder.y, feetToIKPositionSpeed);
            currentIKPosition.y = yLerp;

            currentIKPosition = transform.TransformPoint(currentIKPosition);

            anim.SetIKRotation(foot, rotationIKHolder);
        }

        anim.SetIKPosition(foot, currentIKPosition);

        Debug.DrawRay(currentIKPosition, Vector3.up);
    }

    private void MovePelvisHeight()
    {
        if (rightFootIKPosition == Vector3.zero || leftFootIKPosition == Vector3.zero || lastLeftFootPositionY == 0)
        {
            lastPelvisPositionY = anim.bodyPosition.y;
            return;
        }

        float lOffsetPosition = leftFootIKPosition.y - transform.position.y - pelvisHeight;
        float rOffsetPosition = rightFootIKPosition.y - transform.position.y - pelvisHeight;

        float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

        Vector3 newPelvisPosition = anim.bodyPosition + Vector3.up * totalOffset;
        newPelvisPosition.y = Mathf.Lerp(lastPelvisPositionY, newPelvisPosition.y, pelvisVerticalSpeed);

        anim.bodyPosition = newPelvisPosition;

        lastPelvisPositionY = newPelvisPosition.y;
    }

    private void FeetPositionSolver(Vector3 fromSkyPosition, ref Vector3 feetIKPositions, ref Quaternion feetIKRotations)
    {
        RaycastHit feetOutHit;

        if (Physics.Raycast(fromSkyPosition, Vector3.down, out feetOutHit, raycastDownDistance, environment))
        {
            feetIKPositions = fromSkyPosition;
            feetIKPositions.y = feetOutHit.point.y + ankleHeight;
            feetIKRotations = Quaternion.FromToRotation(Vector3.up, feetOutHit.normal) * transform.rotation;
            return;
        }

        feetIKPositions = Vector3.zero;
    }

    private void AdjustFeetTarget(ref Vector3 footPosition, HumanBodyBones foot)
    {
        footPosition = anim.GetBoneTransform(foot).position;
        footPosition.y = transform.position.y + heightToStartRaycast;
    }
}