using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> Change current state at animation start / end
public class ChangeCurrentState : StateMachineBehaviour
{
    // value that the current state should change to
    public int value;
    // should the variable be changed on state enter or on state exit
    public bool onEnter;

    ///<summary> if on enter will set the integer </summary>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onEnter) animator.SetInteger(Parameters.currentState, value);
    }

    ///<summary> if on exit will set the integer </summary>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!onEnter) animator.SetInteger(Parameters.currentState, value);
    }
}
