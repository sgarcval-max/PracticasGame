using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 3f;
    public int damage = 1;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si choca con un pez le hacemos daño
        FishEnemy fish = other.GetComponent<FishEnemy>();
        if (fish != null)
        {
            fish.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

