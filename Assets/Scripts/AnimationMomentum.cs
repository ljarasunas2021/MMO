using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMomentum : StateMachineBehaviour
{
    [Header("TargetSpeed")]
    public bool useSetSpeed;
    public float speed;
    public float previousSpeedMultiplier;

    [Header("Momentum")]
    public bool useMomentum;
    public float accelerationSmoothTime, decelerationSmoothTime;

    private float smoothVelocity;
    private float targetSpeed;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (useSetSpeed) targetSpeed = speed;
        else targetSpeed = animator.GetFloat(Parameters.currentSpeed) * previousSpeedMultiplier;

        if (!useMomentum) animator.SetFloat(Parameters.currentSpeed, speed);
    }

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
}
