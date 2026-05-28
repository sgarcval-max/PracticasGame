using UnityEngine;

public class FishEnemy : MonoBehaviour
{
    public float speed = 3f;
    public int health = 3;
    public int damage = 1;
    public float tameChance = 0.1f;
    public GameObject tameableFishPrefab;
    public FishType fishType;

    // Si este pez es domesticable desde el principio
    private bool isTameable = false;

    private Transform target;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color normalColor;

    // Color que tendrá el pez domesticable
    public Color tameableColor = new Color(1f, 0.9f, 0f); // Dorado

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        normalColor = sr.color;

        GameObject player = GameObject.Find("Diver");
        if (player != null)
            target = player.transform;
    }

    // Llamado desde el WaveManager al spawnear
    public void SetTameable(bool tameable)
    {
        isTameable = tameable;

        if (isTameable)
        {
            // Cambiamos el color para que se vea diferente
            sr.color = tameableColor;
        }
        else
        {
            sr.color = normalColor;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        // Voltear sprite según dirección sin rotar el objeto
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Debug.Log("Direction.x: " + direction.x + " FlipX: " + sr.flipX);
            if (direction.x > 0)
                sr.flipX = true;  // Va derecha, volteamos porque el sprite mira izquierda
            else if (direction.x < 0)
                sr.flipX = false; // Va izquierda, normal
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        DiverHealth diver = other.gameObject.GetComponent<DiverHealth>();
        if (diver != null)
        {
            diver.TakeDamage(damage);

            // Activar animación de ataque
            FishAnimator fishAnimator = GetComponent<FishAnimator>();
            if (fishAnimator != null)
                fishAnimator.TriggerAttack();

            // Volver a move después de un momento
            StartCoroutine(StopAttackAfterDelay());
        }
    }

    System.Collections.IEnumerator StopAttackAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        FishAnimator fishAnimator = GetComponent<FishAnimator>();
        if (fishAnimator != null)
            fishAnimator.StopAttack();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Si es domesticable spawneamos el pez domesticable directamente
        if (isTameable && tameableFishPrefab != null)
        {
            GameObject tameable = Instantiate(tameableFishPrefab, transform.position, Quaternion.identity);
            TameableFish tf = tameable.GetComponent<TameableFish>();
            if (tf != null)
            {
                tf.fishType = fishType;
            }
        }
        else if (!isTameable && tameableFishPrefab != null && Random.value <= tameChance)
        {
            // Si no era domesticable hay una pequeña probabilidad de que suelte uno igual
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
}