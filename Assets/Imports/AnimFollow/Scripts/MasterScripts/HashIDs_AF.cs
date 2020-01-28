using UnityEngine;
using System.Collections;

namespace AnimFollow
{
    public class HashIDs_AF : MonoBehaviour
    {
        // Add this script to the master

        public readonly int version = 7; // The version of this script

        // Here we store the hash tags for various strings used in our animators.
        [HideInInspector]
        public int dyingState, locomotionState, deadBool, speedFloat, sneakingBool, frontTrigger, backTrigger, frontMirrorTrigger, backMirrorTrigger, idle, getupFront, getupBack, getupFrontMirror, getupBackMirror, anyStateToGetupFront, anyStateToGetupBack, anyStateToGetupFrontMirror, anyStateToGetupBackMirror;

        void Awake()
        {
            dyingState = Animator.StringToHash("Base Layer.Dying");
            locomotionState = Animator.StringToHash("Base Layer.Locomotion");
            deadBool = Animator.StringToHash("Dead");
            sneakingBool = Animator.StringToHash("Sneaking");

            idle = Animator.StringToHash("Base Layer.Idle");

            // These are used by the RagdollControll script and must exist exactly as below
            speedFloat = Animator.StringToHash("Speed");

            frontTrigger = Animator.StringToHash("FrontTrigger");
            backTrigger = Animator.StringToHash("BackTrigger");
            frontMirrorTrigger = Animator.StringToHash("FrontMirrorTrigger");
            backMirrorTrigger = Animator.StringToHash("BackMirrorTrigger");

            getupFront = Animator.StringToHash("Base Layer.GetupFront");
            getupBack = Animator.StringToHash("Base Layer.GetupBack");
            getupFrontMirror = Animator.StringToHash("Base Layer.GetupFronMirror");
            getupBackMirror = Animator.StringToHash("Base Layer.GetupBackMirror");

            anyStateToGetupFront = Animator.StringToHash("Entry -> Base Layer.GetupFront");
            anyStateToGetupBack = Animator.StringToHash("Entry -> Base Layer.GetupBack");
            anyStateToGetupFrontMirror = Animator.StringToHash("Entry -> Base Layer.GetupFrontMirror");
            anyStateToGetupBackMirror = Animator.StringToHash("Entry -> Base Layer.GetupBackMirror");
        }
    }
}
