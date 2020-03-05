using Mirror;

///<summary> handles player health </summary>
public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public float health;
    private InputHandler inputHandler;

    private void Start() { inputHandler = GetComponent<InputHandler>(); }

    public void SubtractHealth(float amount)
    {
        health -= amount;
        if (health <= 0) inputHandler.SetDead(true);
    }
}
