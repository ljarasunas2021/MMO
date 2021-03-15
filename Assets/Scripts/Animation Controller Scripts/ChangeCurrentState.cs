using MMO.Player;
using UnityEngine;

namespace MMO.Animation
{
    ///<summary> Changes the current state parameter of the animator </summary>
    public class ChangeCurrentState : StateMachineBehaviour
    {
        [Header("Enter")]
        // should the current state be changed when the animator starts playing this animation
        public bool onEnter;
        // if onEnter, then this will be the current state's value when the animator starts playing this animation
        public int enterValue;

        [Header("Exit")]
        // should the current state be changed when the animator stops playing this animation
        public bool onExit;
        // if onExit, then this will be the current state's value when the animator stops playing this animation
        public int exitValue;

        ///<summary> When the animation starts playing, if onEnter, this function sets the current state to the enterValue. This function is called automatically by Unity. </summary>
        /// <param name="animator"> animator on root gameobject </param>
        /// <param name="stateInfo"> information about the animator state </param>
        /// <param name="layerIndex"> the layer of this animator state </param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onEnter) animator.SetInteger(PlayerAnimParameters.currentState, enterValue);
        }

        ///<summary> When the animation stops playing, if onExit, this function sets the current state to the exitValue. This function is called automatically by Unity. </summary>
        /// <param name="animator"> animator on root gameobject </param>
        /// <param name="stateInfo"> information about the animator state </param>
        /// <param name="layerIndex"> the layer of this animator state </param>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onExit) animator.SetInteger(PlayerAnimParameters.currentState, exitValue);
        }
    }
}
