using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary> Set speed according to animation being played </summary>
public class AnimationMomentum : StateMachineBehaviour
{
    #region Variables
    [Header("TargetSpeed")]
    // use predefined speed
    public bool useSetSpeed;
    // predefined speed - speed animation will move at
    public float speed;
    // use previous speed times this multiplier
    public float previousSpeedMultiplier;

    [Header("Momentum")]
    // use momentum meaning that previous speed will carry to your current speed
    public bool useMomentum;
    // time it will take to accelerate, decelerate
    public float accelerationSmoothTime, decelerationSmoothTime;

    // velocity that the momentum uses
    private float smoothVelocity;
    // target speed while anim is playing
    private float targetSpeed;
    #endregion

    #region SetSpeed
    /// <summary> Set the target speed </summary>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (useSetSpeed) targetSpeed = speed;
        else targetSpeed = animator.GetFloat(Parameters.currentSpeed) * previousSpeedMultiplier;

        if (!useMomentum) animator.SetFloat(Parameters.currentSpeed, speed);
    }

    /// <summary> If use momentum slerp between current and target speed </summary>
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (useMomentum)
        {
            float currentSpeed = animator.GetFloat(Parameters.currentSpeed);
            if (speed - currentSpeed > 0)
            {
                animator.SetFloat(Parameters.currentSpeed, Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothVelocity, accelerationSmoothTime));
            }
            else
            {
                animator.SetFloat(Parameters.currentSpeed, Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothVelocity, decelerationSmoothTime));
            }
        }
    }
    #endregion
}
