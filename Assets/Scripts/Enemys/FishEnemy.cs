using UnityEngine;

public class FishEnemy : MonoBehaviour
{
    [Header("Tameable")]
    public RuntimeAnimatorController tameableAnimator;

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
            // Cambiamos el Animator Controller al tameable
            Animator anim = GetComponent<Animator>();
            if (anim != null && tameableAnimator != null)
                anim.runtimeAnimatorController = tameableAnimator;
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

            FishAnimator fishAnimator = GetComponent<FishAnimator>();
            if (fishAnimator != null)
                fishAnimator.TriggerAttack();

            StartCoroutine(StopAttackAfterDelay());
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        // Cuando deja de tocar al player para la animación de ataque
        DiverHealth diver = other.gameObject.GetComponent<DiverHealth>();
        if (diver != null)
        {
            FishAnimator fishAnimator = GetComponent<FishAnimator>();
            if (fishAnimator != null)
                fishAnimator.StopAttack();
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
        // Solo suelta pez domesticable si era tameable
        if (isTameable && tameableFishPrefab != null)
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
            waveManager.OnFishDied();

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