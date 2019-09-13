using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCurrentState : StateMachineBehaviour
{
    public int value;
    public bool onEnter;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onEnter) animator.SetInteger("CurrentState", value);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!onEnter) animator.SetInteger("CurrentState", value);
    }
}
