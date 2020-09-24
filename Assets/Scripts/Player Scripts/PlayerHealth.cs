using Mirror;

///<summary> handles player health </summary>
public class PlayerHealth : NetworkBehaviour
{
    // health
    [SyncVar]
    public float health;

    // max health
    public float maxHealth;
    // is this a player
    public bool isPlayer = true;
    
    // player scripts
    private InputHandler inputHandler;
    private HealthBar healthBar;

    /// <summary> Init vars </summary>
    private void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        healthBar = HealthBar.instance;
        healthBar.Initialize(gameObject, health);
        maxHealth = health;
    }

    /// <summary> Subtract health from player </summary>
    /// <param name="amount"> amount of health to subtract </param>
    public void SubtractHealth(float amount)
    {
        health -= amount;
        if (isPlayer)
        {
            healthBar.SetHealth(health);
            if (health <= 0)
            {
                inputHandler.SetDead(true);
                foreach (EnemyAI1 enemyAI in FindObjectsOfType<EnemyAI1>())
                {
                    enemyAI.PlayerDead();
                }
            }
        }
        else
        {
            if (health <= 0) GetComponent<EnemyAI1>().EnemyDead();
        }
    }
}
