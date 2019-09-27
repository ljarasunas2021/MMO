using UnityEngine;

///<Summary> Add momentum between locomotion states </summary>
public class LocomotionMomentum : StateMachineBehaviour
{
    #region Variables
    [Header("Values")]
    // locomotion blend value when playing idle, walking, and running animations
    public float idleVal, walkVal, runVal;
    [Header("Speeds")]
    // speed player moves by when in idle, walking, and running animations
    public float idleSpeed, walkSpeed, runSpeed;
    [Header("Smooth Times")]
    // how fast player accelerates, decelerates
    public float accelerationSmoothTime, decelerationSmoothTime;

    //velocity of the change in speed
    private float smoothVelocity;
    // target speed velocity should reach
    private float targetSpeed;
    #endregion

    #region SetSpeed
    ///<summary> Set speed of player based on locomotion blend value
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
    #endregion
}
