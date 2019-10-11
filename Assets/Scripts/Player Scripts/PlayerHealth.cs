using Mirror;

///<summary> handles player health </summary>
public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    // health of player
    public double health;

    // input handler script
    private InputHandler inputHandler;

    #region Initialize
    ///<summary> initialize variables </summary>
    private void Start() { inputHandler = GetComponent<InputHandler>(); }
    #endregion

    #region SubtractHealth
    ///<summary> subtract health from player </summary>
    public void SubtractHealth(double amount)
    {
        health -= amount;
        if (health <= 0) inputHandler.SetDead(true);
    }
    #endregion
}
