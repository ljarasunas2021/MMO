using UnityEngine;
using System.Collections.Generic;

///<Summary> Controls the speed of the player while playing the locomotion state. </summary>
public class LocomotionMomentum : StateMachineBehaviour
{
    // array of every locomotion state
    public LocomotionState[] states;

    [Header("Smooth Times")]
    // how fast the player should accelerate, decelerate
    public float accelerationSmoothTime, decelerationSmoothTime;

    //velocity of the change in speed
    private float smoothVelocity;
    // speed the player should reach
    private float targetSpeed;

    // a dictionary which for every double from -1 to 1 defines a BoundsAndSpeed. This dictionary is used to find the appropriate z speed for a player's locomotion blend value
    private Dictionary<double, BoundsAndSpeed> locomotionBlendDict = new Dictionary<double, BoundsAndSpeed>();
    // a dictionary which for every double from -1 to 1 defines a BoundsAndSpeed. This dictionary is used to find the appropriate x speed for a player's locomotion dir value
    private Dictionary<double, BoundsAndSpeed> locomotionDirDict = new Dictionary<double, BoundsAndSpeed>();

    /// <summary> Inits the locmotionBlend and locomotionDir dictionaries </summary>
    private void Awake()
    {
        for (double i = -1; i < 1; i += 0.1)
        {
            float upperBound = 1;
            float lowerBound = -1;
            float upperBoundSpeed = 0;
            float lowerBoundSpeed = 0;
            foreach (LocomotionState state in states)
            {
                float currentValue = state.locomotionBlendValue;

                if (currentValue < upperBound && currentValue > i)
                {
                    upperBound = currentValue;
                    upperBoundSpeed = state.speed.y;
                }
                else if (currentValue > lowerBound && currentValue < i)
                {
                    lowerBound = currentValue;
                    lowerBoundSpeed = state.speed.y;
                }
                else if (currentValue == i)
                {
                    lowerBound = (float)i;
                    lowerBoundSpeed = state.speed.y;
                    upperBound = (float)i;
                    upperBoundSpeed = state.speed.y;
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
            foreach (LocomotionState motion in states)
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

    ///<summary> Sets the player's speed. First find the current locomotion blend and dir values, then using the dictionaries find the appropriate speed based off of the closest motions. This function is called automatically by Unity.  </summary>
    /// <param name="animator"> animator on root gameobject </param>
    /// <param name="stateInfo"> information about the animator state </param>
    /// <param name="layerIndex"> the layer of this animator state </param>
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

    ///<summary> Convert a float to a double </summary>
    ///<param name = "i"> float to be converted </param>
    ///<returns> a double </returns>
    private double ConvertToDouble(float i)
    {
        return Mathf.Round(i * 10) / 10;
    }
}

[System.Serializable]
///<summary> locomotion state with its corresponding locomotion variables </summary>
public class LocomotionState
{
    // name of the locomotion state
    public string name;
    // y-axis value of the blend tree for this state
    public float locomotionBlendValue;
    // x-axis value of the blend tree for this state
    public float locomotionDirValue;
    // speed at which the player should be moving at this state
    public Vector2 speed;
}

/// <summary> Given a double between -1 and 1, this class stores variables to find a player's speed through interpolation </summary>
public class BoundsAndSpeed
{
    // the largest locomotionBlend or locomotionDir value (of a motion) less than the double
    public float lowerBound;
    // the speed of the lowerBound's motion
    public float lowerBoundSpeed;
    // the smallest locomotionBlend or locomotionDir value (of a motion) greater than the double
    public float upperBound;
    // the speed of the upperBound's motion
    public float upperBoundSpeed;

    // constructor
    public BoundsAndSpeed(float lowerBound, float lowerBoundSpeed, float upperBound, float upperBoundSpeed)
    {
        this.lowerBound = lowerBound;
        this.lowerBoundSpeed = lowerBoundSpeed;
        this.upperBound = upperBound;
        this.upperBoundSpeed = upperBoundSpeed;
    }
}
