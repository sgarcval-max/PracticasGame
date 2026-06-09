using UnityEngine;

public class PufferfishAbility : FishAbility
{
    public float explosionRadius = 3f;
    public int explosionDamage = 2;
    public float pushForce = 10f;
    public GameObject explosionEffectPrefab;

    void Awake()
    {
        abilityName = "Explosion";
        cooldown = 8f;
        duration = 0f;
        fishType = FishType.Pufferfish;
    }

    protected override void Activate()
    {
        Debug.Log("Explosion!");

        // Spawneamos el efecto de explosión en la posición del buzo
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            FishEnemy fish = hit.GetComponent<FishEnemy>();
            if (fish != null)
            {
                fish.TakeDamage(explosionDamage);

                Rigidbody2D fishRb = hit.GetComponent<Rigidbody2D>();
                if (fishRb != null)
                {
                    Vector2 pushDir = (hit.transform.position - transform.position).normalized;
                    fishRb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
