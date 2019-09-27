using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionMomentum : StateMachineBehaviour
{
    public float idleVal, walkVal, runVal;
    public float idleSpeed, walkSpeed, runSpeed;
    public float accelerationSmoothTime, decelerationSmoothTime;

    private float smoothVelocity;
    private float targetSpeed;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        float locomotionBlendVal = animator.GetFloat(Parameters.locomotionBlend);

        if (locomotionBlendVal < walkVal)
        {
            animator.SetFloat(Parameters.currentSpeed, ((walkSpeed - idleSpeed) / (walkVal - idleVal)) * (locomotionBlendVal - idleVal) + idleSpeed);
        }
        else
        {
            animator.SetFloat(Parameters.currentSpeed, ((runSpeed - walkSpeed) / (runVal - walkVal)) * (locomotionBlendVal - walkVal) + walkSpeed);
        }
    }
}
