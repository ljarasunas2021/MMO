using UnityEngine;
using MMO.Player;

///<summary> Change if the player can rotate when the animation starts playing </summary>
public class ChangeCanRotate : StateMachineBehaviour
{
    [Header("Enter")]
    // should the variable be changed on state enter
    public bool onEnter;
    // value that the rotation should change to when entering
    public bool enterValue;

    [Header("Exit")]
    // should the variable be changed on state exit
    public bool onExit;
    // value that the rotation should change to when exiting
    public bool exitValue;

    ///<summary> if onEnter, this function will set the player's rotation to the appropriate value when the animator starts playing this animation. This function is called automatically by Unity. </summary>
    /// <param name="animator"> animator on root gameobject </param>
    /// <param name="stateInfo"> information about the animator state </param>
    /// <param name="layerIndex"> the layer of this animator state </param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onEnter) animator.SetBool(PlayerAnimParameters.canRotate, enterValue);
    }

    ///<summary> if onExit, this function will set the player's rotation to the appropriate value when the animator stops playing this animation. This function is called automatically by Unity. </summary>
    /// <param name="animator"> animator on root gameobject </param>
    /// <param name="stateInfo"> information about the animator state </param>
    /// <param name="layerIndex"> the layer of this animator state </param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onExit) animator.SetBool(PlayerAnimParameters.canRotate, exitValue);
    }
}