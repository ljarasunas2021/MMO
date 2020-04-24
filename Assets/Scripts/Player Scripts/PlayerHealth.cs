using Mirror;

///<summary> handles player health </summary>
public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public float health;
    public bool isPlayer = true;
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
