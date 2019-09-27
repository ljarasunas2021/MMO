using UnityEngine;

///<summary> Change rotation at animation start / end
public class ChangeCanRotate : StateMachineBehaviour
{
    #region Variables
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
    #endregion

    #region ChangeVars
    ///<summary> if on enter will set the bool </summary>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onEnter) animator.SetBool(Parameters.canRotate, enterValue);
    }

    ///<summary> if on exit will set the bool </summary>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onExit) animator.SetBool(Parameters.canRotate, exitValue);
    }
    #endregion
}