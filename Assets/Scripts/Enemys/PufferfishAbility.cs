using UnityEngine;

public class PufferfishAbility : FishAbility
{
    // Radio de la explosión
    public float explosionRadius = 3f;

    // Daño que hace la explosión
    public int explosionDamage = 2;

    // Fuerza con la que empuja a los peces
    public float pushForce = 10f;

    void Awake()
    {
        abilityName = "Explosion";
        cooldown = 8f;
        duration = 0f;
    }

    protected override void Activate()
    {
        Debug.Log("Explosion!");

        // Buscamos todos los colliders en el radio de explosion
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            // Si es un pez enemigo le hacemos daño
            FishEnemy fish = hit.GetComponent<FishEnemy>();
            if (fish != null)
            {
                fish.TakeDamage(explosionDamage);

                // Empujamos el pez hacia afuera
                Rigidbody2D fishRb = hit.GetComponent<Rigidbody2D>();
                if (fishRb != null)
                {
                    Vector2 pushDir = (hit.transform.position - transform.position).normalized;
                    fishRb.AddForce(pushDir * pushForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    // Dibuja el radio en el editor para verlo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
