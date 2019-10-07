using UnityEngine;

public class ChangeLayerWeight : StateMachineBehaviour
{
    #region Variables
    [Header("Enter")]
    public bool onEnter;
    public float enterLayerWeight;

    [Header("Exit")]
    public bool onExit;
    public float exitLayerWeight;
    #endregion

    #region Layer Weight
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onEnter) animator.SetLayerWeight(layerIndex, enterLayerWeight);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onExit) animator.SetLayerWeight(layerIndex, exitLayerWeight);
    }
    #endregion
}
