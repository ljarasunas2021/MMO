using Mirror;

///<summary> handles player health </summary>
public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public float health;
    private InputHandler inputHandler;
    private HealthBar healthBar;

    private void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        healthBar = FindObjectOfType<HealthBar>();
        healthBar.Initialize(gameObject, health);
    }

    public void SubtractHealth(float amount)
    {
        health -= amount;
        healthBar.TakeDamage(health);
        if (health <= 0) inputHandler.SetDead(true);
    }
}
