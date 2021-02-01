using UnityEngine;

///<summary> Sets the player's speed based on the animation being played. </summary>
public class AnimationMomentum : StateMachineBehaviour
{
    [Header("TargetSpeed")]
    // should the player's speed be set to a constant 
    public bool useSetSpeed;
    // if useSetSpeed, the player's speed will be this constant
    public float speed;
    // if !useSetSpeed, the player's speed will be set to the player's current speed times this multiplier
    public float previousSpeedMultiplier;

    [Header("Momentum")]
    // smoothly change the speed of the player
    public bool useMomentum;
    // if useMomentum, the time it will take to accelerate, decelerate
    public float accelerationSmoothTime, decelerationSmoothTime;

    // velocity variable that the momentum uses
    private float smoothVelocity;
    // the target speed that the player will interpolate towards
    private float targetSpeed;

    /// <summary> When this animation starts playing, this function sets the target speed and sets the player's speed if the player isn't using momentum. This function is called automatically by Unity. </summary>
    /// <param name="animator"> animator on root gameobject </param>
    /// <param name="stateInfo"> information about the animator state </param>
    /// <param name="layerIndex"> the layer of this animator state </param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (useSetSpeed) targetSpeed = speed;
        else targetSpeed = animator.GetFloat(PlayerAnimParameters.currentSpeedZ) * previousSpeedMultiplier;

        if (!useMomentum) animator.SetFloat(PlayerAnimParameters.currentSpeedZ, targetSpeed);
    }

    /// <summary> If useMomentum, interpolate between the current and target speed. This function is called automatically by Unity. </summary>
    /// <param name="animator"> animator on root gameobject </param>
    /// <param name="stateInfo"> information about the animator state </param>
    /// <param name="layerIndex"> the layer of this animator state </param>
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (useMomentum)
        {
            float currentSpeed = animator.GetFloat(PlayerAnimParameters.currentSpeedZ);
            if (speed - currentSpeed > 0)
            {
                animator.SetFloat(PlayerAnimParameters.currentSpeedZ, Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothVelocity, accelerationSmoothTime));
            }
            else
            {
                animator.SetFloat(PlayerAnimParameters.currentSpeedZ, Mathf.SmoothDamp(currentSpeed, targetSpeed, ref smoothVelocity, decelerationSmoothTime));
            }
        }
    }
}
