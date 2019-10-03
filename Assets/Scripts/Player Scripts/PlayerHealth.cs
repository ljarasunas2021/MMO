using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public double health;

    private Movement movement;

    private void Start()
    {
        movement = GetComponent<Movement>();
    }

    public void SubtractHealth(double amount)
    {
        health -= amount;
        if (health <= 0) movement.SetDead(true);
    }
}
