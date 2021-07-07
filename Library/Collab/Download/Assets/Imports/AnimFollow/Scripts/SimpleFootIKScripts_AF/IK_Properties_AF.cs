using System.Collections;
using UnityEngine;

namespace AnimFollow
{
    public partial class SimpleFootIK_AF
    {
        [Header("Ragdol")]
        [SerializeField] [Tooltip("Ragdoll transform")] private Transform ragdoll;

        [SerializeField] [Tooltip("Layers to be ignored")] private string[] ignoreLayers = { "Water" };

        [Header("IK vars")]
        [Range(0f, 1f)] [SerializeField] [Tooltip("Foot IK weight")] private float footIKWeight = 1f;
        [Range(4f, 20f)] [SerializeField] [Tooltip("Character must not be higher above ground than this.")] private float raycastLength = 5f;
        [Range(.2f, .9f)] [SerializeField] [Tooltip("Max height that player can step")] private float maxStepHeight = .5f;
        [Range(0f, 1f)] [SerializeField] [Tooltip("Foot IK not active on inclines steeper than arccos(maxIncline)")] private float maxIncline = .8f;

        [Header("Lerps")]
        [Range(1f, 100f)] [SerializeField] [Tooltip("Lerp smoothing of foot normals")] private float footNormalLerp = 40f;
        [Range(1f, 100f)] [SerializeField] [Tooltip("Lerp smoothing of foot position")] private float footTargetLerp = 40f;
        [Range(1f, 100f)] [SerializeField] [Tooltip("Lerp smoothing of transform following terrain")] private float transformYLerp = 20f;

        [HideInInspector] public bool userNeedsToFixStuff = false;

        [HideInInspector] public float extraYLerp = 1f;
        private LayerMask layerMask;
        private float footHeight; // Is set in Awake as the difference between foot position and transform.position. At Awake the character's transform.position must be level with feet soles.

        private Animator animator;
        private AnimFollow.AnimFollow_AF animFollow;
        private float deltaTime;

        private RaycastHit raycastHitLeftFoot;
        private RaycastHit raycastHitRightFoot;
        private RaycastHit raycastHitToe;

        private Transform leftToe;
        private Transform leftFoot;
        private Transform leftCalf;
        private Transform leftThigh;
        private Transform rightToe;
        private Transform rightFoot;
        private Transform rightCalf;
        private Transform rightThigh;

        private Quaternion leftFootRotation;
        private Quaternion rightFootRotation;

        private Vector3 leftFootTargetPos;
        private Vector3 leftFootTargetNormal;
        private Vector3 lastLeftFootTargetPos;
        private Vector3 lastLeftFootTargetNormal;
        private Vector3 rightFootTargetPos;
        private Vector3 rightFootTargetNormal;
        private Vector3 lastRightFootTargetPos;
        private Vector3 lastRightFootTargetNormal;

        private Vector3 footForward;

        private float thighLength, thighLengthSquared;
        private float calfLength, calfLengthSquared;
        private float reciDenominator;
    }
}
