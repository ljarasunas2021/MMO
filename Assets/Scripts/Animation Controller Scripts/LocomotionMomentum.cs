using UnityEngine;
using System.Collections.Generic;

///<Summary> Add momentum between locomotion states </summary>
public class LocomotionMomentum : StateMachineBehaviour
{
    #region Variables
    // list of motions with their speed
    public LocomotionMotion[] motions;

    [Header("Smooth Times")]
    // how fast player accelerates, decelerates
    public float accelerationSmoothTime, decelerationSmoothTime;

    //velocity of the change in speed
    private float smoothVelocity;
    // target speed velocity should reach
    private float targetSpeed;

    // a dictionary that every double from -1 to 1 and gives an upper and lower bound for locomotion blend values with the corresponding speeds to the bounds
    private Dictionary<double, BoundsAndSpeed> locomotionBlendDict = new Dictionary<double, BoundsAndSpeed>();
    // a dictionary that every double from -1 to 1 and gives an upper and lower bound for locomotion direction values with the corresponding speeds to the bounds
    private Dictionary<double, BoundsAndSpeed> locomotionDirDict = new Dictionary<double, BoundsAndSpeed>();
    #endregion

    #region Initialize
    /// <summary> Set up dictionaries above </summary>
    private void Awake()
    {
        for (double i = -1; i < 1; i += 0.1)
        {
            float upperBound = 1;
            float lowerBound = -1;
            float upperBoundSpeed = 0;
            float lowerBoundSpeed = 0;
            foreach (LocomotionMotion motion in motions)
            {
                float currentValue = motion.locomotionBlendValue;

                if (currentValue < upperBound && currentValue > i)
                {
                    upperBound = currentValue;
                    upperBoundSpeed = motion.speed.y;
                }
                else if (currentValue > lowerBound && currentValue < i)
                {
                    lowerBound = currentValue;
                    lowerBoundSpeed = motion.speed.y;
                }
                else if (currentValue == i)
                {
                    lowerBound = (float)i;
                    lowerBoundSpeed = motion.speed.y;
                    upperBound = (float)i;
                    upperBoundSpeed = motion.speed.y;
                }
            }
            i = ConvertToDouble((float)i);
            locomotionBlendDict.Add((double)i, new BoundsAndSpeed(lowerBound, lowerBoundSpeed, upperBound, upperBoundSpeed));
        }

        for (double i = -1; i < 1; i += 0.1)
        {
            float upperBound = 1;
            float lowerBound = -1;
            float upperBoundSpeed = 0;
            float lowerBoundSpeed = 0;
            foreach (LocomotionMotion motion in motions)
            {
                float currentValue = motion.locomotionDirValue;

                if (currentValue < upperBound && currentValue > i)
                {
                    upperBound = currentValue;
                    upperBoundSpeed = motion.speed.x;
                }
                else if (currentValue > lowerBound && currentValue < i)
                {
                    lowerBound = currentValue;
                    lowerBoundSpeed = motion.speed.x;
                }
                else if (currentValue == i)
                {
                    lowerBound = (float)i;
                    lowerBoundSpeed = motion.speed.x;
                    upperBound = (float)i;
                    upperBoundSpeed = motion.speed.x;
                }
            }
            i = ConvertToDouble((float)i);
            locomotionDirDict.Add((double)i, new BoundsAndSpeed(lowerBound, lowerBoundSpeed, upperBound, upperBoundSpeed));
        }
    }
    #endregion

    #region SetSpeed
    ///<summary> Set speed of player based on locomotion blend and dir values </summary>
    // idea is that you find the target locomotion blend and dir value, find most nearby motions and linearly find target speed based on those nearby motions and the speeds of those motions
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        float locomotionBlendVal = animator.GetFloat(Parameters.locomotionBlend);
        float locomotionDirVal = animator.GetFloat(Parameters.locomotionDir);

        BoundsAndSpeed zBound = locomotionBlendDict[ConvertToDouble(locomotionBlendVal)];
        float zSpeed = (zBound.lowerBound != zBound.upperBound) ? (((locomotionBlendVal - zBound.lowerBound) / (zBound.upperBound - zBound.lowerBound)) * (zBound.upperBoundSpeed - zBound.lowerBoundSpeed)) + zBound.lowerBoundSpeed : zBound.upperBoundSpeed;

        BoundsAndSpeed xBound = locomotionDirDict[ConvertToDouble(locomotionDirVal)];
        float xSpeed = (xBound.lowerBound != xBound.upperBound) ? (((locomotionDirVal - xBound.lowerBound) / (xBound.upperBound - xBound.lowerBound)) * (xBound.upperBoundSpeed - xBound.lowerBoundSpeed)) + xBound.lowerBoundSpeed : xBound.upperBoundSpeed;

        animator.SetFloat(Parameters.currentSpeedZ, zSpeed);
        animator.SetFloat(Parameters.currentSpeedX, xSpeed);
    }
    #endregion

    #region ConvertToDouble
    ///<summary> Convert float to double </summary>
    ///<param name = "i"> float to be converted </param>
    private double ConvertToDouble(float i)
    {
        return Mathf.Round(i * 10) / 10;
    }
    #endregion
}

#region LocomotionMomentum
[System.Serializable]
///<summary> motion with corresponding speed </summary>
public class LocomotionMotion
{
    public string name;
    public float locomotionBlendValue;
    public float locomotionDirValue;
    public Vector2 speed;
}
#endregion

#region BoundsAndSpeed
/// <summary> used to find upper and lower bound around a double from -1 to 1 </summary>
public class BoundsAndSpeed
{
    public float lowerBound, lowerBoundSpeed, upperBound, upperBoundSpeed;

    public BoundsAndSpeed(float lowerBound, float lowerBoundSpeed, float upperBound, float upperBoundSpeed)
    {
        this.lowerBound = lowerBound;
        this.lowerBoundSpeed = lowerBoundSpeed;
        this.upperBound = upperBound;
        this.upperBoundSpeed = upperBoundSpeed;
    }
}
#endregion
