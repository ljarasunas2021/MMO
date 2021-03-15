using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCapFight : Action1
{
    private PlayerHealth playerHealth, enemyHealth;
    public Transform weaponSpawnPosition;
    public GameObject weapon;
    public GameObject dialogueBox;

    public override IEnumerator Execute()
    {
        enemyHealth = GetComponent<PlayerHealth>();
        playerHealth = GameObject.Find("Player_0(Clone)").GetComponent<PlayerHealth>();

        dialogueBox.SetActive(false);

        Instantiate(weapon, weaponSpawnPosition.position, Quaternion.identity);

        GetComponent<EnemyAI1>().StartFiring();

        while (enemyHealth.health > 0 && playerHealth.health > 0)
        {
            yield return 0;
        }

        if (enemyHealth.health <= 0)
        {

        }

        if (playerHealth.health <= 0) playerHealth.health = playerHealth.maxHealth;

        dialogueBox.SetActive(true);
    }
}
