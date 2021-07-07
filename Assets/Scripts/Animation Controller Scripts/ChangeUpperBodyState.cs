using UnityEngine;

///<summary> Change current state at animation start / end
public class ChangeUpperBodyState : StateMachineBehaviour
{
    #region Variables
    [Header("Enter")]
    // should the variable be changed on state enter
    public bool onEnter;
    // value that the current state should change to when entering
    public int enterValue;
    //public bool changeTargetValueToo;

    [Header("Exit")]
    // should the variable be changed on state exit
    public bool onExit;
    // value that the current state should change to when exiting
    public int exitValue;
    #endregion

    #region ChangeVariables
    ///<summary> if on enter will set the integer </summary>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onEnter) animator.SetInteger(Parameters.upperBodyState, enterValue);
    }

    ///<summary> if on exit will set the integer </summary>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onExit) animator.SetInteger(Parameters.upperBodyState, exitValue);
    }
    #endregion
}