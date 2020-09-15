using Mirror;

///<summary> handles player health </summary>
public class PlayerHealth : NetworkBehaviour
{
    // health
    [SyncVar]
    public float health;

    // max health
    public float maxHealth;
    // is player?
    public bool isPlayer = true;
    // player scripts
    private InputHandler inputHandler;
    private HealthBar healthBar;

    // init vars
    private void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        healthBar = HealthBar.instance;
        healthBar.Initialize(gameObject, health);
        maxHealth = health;
    }

    // subtract health from player
    public void SubtractHealth(float amount)
    {
        health -= amount;
        if (isPlayer)
        {
            healthBar.TakeDamage(health);
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
            if (health <= 0) GetComponent<EnemyAI1>().Dead();
        }
    }
}
