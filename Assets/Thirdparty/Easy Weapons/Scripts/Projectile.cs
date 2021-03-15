using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public DamageType damageType = DamageType.Direct;                   // The damage type - Direct applys damage directly from the projectile, Explosion lets an instantiated explosion handle damage
    public float damage = 100.0f;                                       // The amount of damage to be applied (only for Direct damage type)
    public float speed = 10.0f;                                         // The speed at which this projectile will move
    public float initialForce = 1000.0f;                                // The force to be applied to the projectile initially
    public float lifetime = 30.0f;                                      // The maximum time (in seconds) before the projectile is destroyed
    public GameObject explosion;                                        // The explosion to be instantiated when this projectile hits something

    private float lifeTimer = 0.0f;
    private Rigidbody rG;                                    // The timer to keep track of how long this projectile has been in existence

    void Start()
    {
        rG = GetComponent<Rigidbody>();
        rG.AddRelativeForce(0, 0, initialForce);
    }

    // Update is called once per frame
    void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime) Explode(transform.position);

        if (initialForce == 0) rG.velocity = transform.forward * speed;
    }

    void OnCollisionEnter(Collision col)
    {
        Hit(col);
    }

    void Hit(Collision col)
    {
        Explode(col.contacts[0].point);

        if (damageType == DamageType.Direct)
        {
            Health health = col.collider.gameObject.GetComponent<Health>();
            if (health != null) health.ChangeHealth(damage);
        }
    }

    void Explode(Vector3 position)
    {
        if (explosion != null)
        {
            Instantiate(explosion, position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}

public enum DamageType
{
    Direct,
    Explosion
}

