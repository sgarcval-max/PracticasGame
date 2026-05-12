using UnityEngine;

public class FishEnemy : MonoBehaviour
{
    public float speed = 3f;
    public int health = 3;
    public int damage = 1;
    public float tameChance = 0.1f;
    public GameObject tameableFishPrefab;
    public FishType fishType;

    private Transform target;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Por defecto el objetivo es el buzo
        GameObject player = GameObject.Find("Diver");
        if (player != null)
            target = player.transform;
    }

    // Permite cambiar el objetivo del pez
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        DiverHealth diver = other.gameObject.GetComponent<DiverHealth>();
        if (diver != null)
        {
            diver.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // Velocidad original guardada
    private float originalSpeed = -1f;

    public void SetSpeed(float newSpeed)
    {
        if (originalSpeed < 0f)
            originalSpeed = speed;
        speed = newSpeed;
    }

    public void RestoreSpeed()
    {
        if (originalSpeed >= 0f)
        {
            speed = originalSpeed;
            originalSpeed = -1f;
        }
    }

    void Die()
    {
        if (tameableFishPrefab != null && Random.value <= tameChance)
        {
            GameObject tameable = Instantiate(tameableFishPrefab, transform.position, Quaternion.identity);
            TameableFish tf = tameable.GetComponent<TameableFish>();
            if (tf != null)
            {
                tf.fishType = fishType;
            }
        }

        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnFishDied();
        }

        Destroy(gameObject);
    }
}