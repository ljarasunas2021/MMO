using MMO.Player;
using UnityEngine;

namespace MMO.Animation
{
    ///<summary> Change the target upper body state on the animtion's enter and/or exit <\summary>
    public class ChangeTargetUpperBodyState : StateMachineBehaviour
    {
        [Header("Enter")]
        // should the target upper body state be changed when the animator starts playing this animation
        public bool onEnter;
        // if onEnter, then this will be the target upper body state when the animator starts playing this animation
        public int enterValue;

        [Header("Exit")]
        // should the target upper body state be changed when the animator stops playing this animation
        public bool onExit;
        // if onExit, then this will be the target upper body state when the animator stops playing this animation
        public int exitValue;

        ///<summary> When the animation starts playing, if onEnter, this function sets the target upper body state to the enterValue. This function is called automatically by Unity. </summary>
        /// <param name="animator"> animator on root gameobject </param>
        /// <param name="stateInfo"> information about the animator state </param>
        /// <param name="layerIndex"> the layer of this animator state </param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onEnter) animator.SetInteger(PlayerAnimParameters.targetUpperBodyState, enterValue);
        }

        ///<summary> When the animation stops playing, if onExit, this function sets the target upper body state to the exitValue. This function is called automatically by Unity. </summary>
        /// <param name="animator"> animator on root gameobject </param>
        /// <param name="stateInfo"> information about the animator state </param>
        /// <param name="layerIndex"> the layer of this animator state </param>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onExit) animator.SetInteger(PlayerAnimParameters.targetUpperBodyState, exitValue);
        }
    }
}
