using UnityEngine;

/// <summary> Change weight of layer on enter or exit </summary>
public class ChangeLayerWeight : StateMachineBehaviour
{
    #region Variables
    [Header("Enter")]
    // if layer weight should change on enter
    public bool onEnter;
    // what the layer weight should change to
    public float enterLayerWeight;

    [Header("Exit")]
    // if layer weight should change on exit
    public bool onExit;
    // what the layer weight should change to
    public float exitLayerWeight;
    #endregion

    #region Layer Weight
    ///<summary> Change layer weight if on enter </summary>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onEnter) animator.SetLayerWeight(layerIndex, enterLayerWeight);
    }

    ///<summary> Change layer weight if on exit </summary>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onExit) animator.SetLayerWeight(layerIndex, exitLayerWeight);
    }
    #endregion
}
