/// <summary> Standard implementation of PID controller. Uses some elementary Calculus concepts, but if you look up PID controller, you should hopefully get a good gauge of what this does. </summary>
[System.Serializable]
public class PID
{
    // proportionality, integral and derivative factor
    public float pFactor, iFactor, dFactor;
    // sum of all errors
    public float integral;
    // the error from the previous time Update was called
    public float lastError;

    /// <summary> Constructor for PID controller</summary>
    /// <param name="pFactor"> proportionality factor</param>
    /// <param name="iFactor"> integral factor </param>
    /// <param name="dFactor"> derivative factor</param>
    /// <returns> the PID object </returns>
    public PID(float pFactor, float iFactor, float dFactor)
    {
        this.pFactor = pFactor;
        this.iFactor = iFactor;
        this.dFactor = dFactor;
    }

    /// <summary> Updates the variables of the PID controller, returns the amount of correction needed </summary>
    /// <param name="setpoint"> target value </param>
    /// <param name="actual"> the current value </param>
    /// <param name="timeFrame"> the amount of time that's passed since Update was last called </param>
    /// <returns> the amount of correction needed </returns>
    public float Update(float setpoint, float actual, float timeFrame)
    {
        float present = setpoint - actual;
        integral += present * timeFrame;
        float deriv = (present - lastError) / timeFrame;
        lastError = present;
        return present * pFactor + integral * iFactor + deriv * dFactor;
    }
}