using UnityEngine;

namespace MMO.Animation
{
    /// <summary> Changes the weight of the animation layer on enter and/or exit </summary>
    public class ChangeLayerWeight : StateMachineBehaviour
    {
        [Header("Enter")]
        // should the layer weight be changed when the animator starts playing this animation
        public bool onEnter;
        // if onEnter, then this will be the layer weight when the animator starts playing this animation
        public float enterLayerWeight;

        [Header("Exit")]
        // should the layer weight be changed when the animator stops playing this animation
        public bool onExit;
        // if onEnter, then this will be the layer weight when the animator stops playing this animation
        public float exitLayerWeight;

        ///<summary> When the animation starts playing, if onEnter, this function sets the layer weight to the enterLayerWeight. This function is called automatically by Unity. </summary>
        /// <param name="animator"> animator on root gameobject </param>
        /// <param name="stateInfo"> information about the animator state </param>
        /// <param name="layerIndex"> the layer of this animator state </param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onEnter) animator.SetLayerWeight(layerIndex, enterLayerWeight);
        }

        ///<summary> When the animation stops playing, if onExit, this function sets the layer weight to the exitLayerWeight. This function is called automatically by Unity. </summary>
        /// <param name="animator"> animator on root gameobject </param>
        /// <param name="stateInfo"> information about the animator state </param>
        /// <param name="layerIndex"> the layer of this animator state </param>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onExit) animator.SetLayerWeight(layerIndex, exitLayerWeight);
        }
    }
}
