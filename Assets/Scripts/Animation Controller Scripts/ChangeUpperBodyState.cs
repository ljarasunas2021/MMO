using UnityEngine;

///<summary> Change the current upper body state on the animtion's enter and/or exit <\summary>
public class ChangeUpperBodyState : StateMachineBehaviour
{
    [Header("Enter")]
    // should the current upper body state be changed when the animator starts playing this animation
    public bool onEnter;
    // if onEnter, then this will be the current upper body state when the animator starts playing this animation
    public int enterValue;
    //public bool changeTargetValueToo;

    [Header("Exit")]
    // should the current upper body state be changed when the animator stops playing this animation
    public bool onExit;
    // if onExit, then this will be the current upper body state when the animator stops playing this animation
    public int exitValue;

    ///<summary> When the animation starts playing, if onEnter, this function sets the current upper body state to the enterValue. This function is called automatically by Unity. </summary>
    /// <param name="animator"> animator on root gameobject </param>
    /// <param name="stateInfo"> information about the animator state </param>
    /// <param name="layerIndex"> the layer of this animator state </param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onEnter) animator.SetInteger(PlayerAnimParameters.upperBodyState, enterValue);
    }

    ///<summary> When the animation stops playing, if onExit, this function sets the current upper body state to the exitValue. This function is called automatically by Unity. </summary>
    /// <param name="animator"> animator on root gameobject </param>
    /// <param name="stateInfo"> information about the animator state </param>
    /// <param name="layerIndex"> the layer of this animator state </param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onExit) animator.SetInteger(PlayerAnimParameters.upperBodyState, exitValue);
    }
}