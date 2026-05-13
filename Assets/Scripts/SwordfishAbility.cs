using UnityEngine;

public class SwordfishAbility : FishAbility
{
    public float dashForce = 25f;
    public float dashDamage = 2;
    public float dashRadius = 0.5f;

    private Rigidbody2D rb;

    void Awake()
    {
        abilityName = "Dash";
        cooldown = 6f;
        duration = 0f;
        rb = GetComponent<Rigidbody2D>();
        fishType = FishType.Pufferfish; // <- añade esta línea
    }

    protected override void Activate()
    {
        Debug.Log("Dash!");

        // Dirección hacia el cursor
        Camera cam = Camera.main;
        Vector3 mousePos = cam.ScreenToWorldPoint(
            UnityEngine.InputSystem.Mouse.current.position.ReadValue()
        );
        mousePos.z = 0f;

        Vector2 dashDir = (mousePos - transform.position).normalized;

        // Empujamos al buzo en esa dirección
        rb.AddForce(dashDir * dashForce, ForceMode2D.Impulse);

        // Dañamos a los peces en el camino
        StartCoroutine(DashDamageCoroutine(dashDir));
    }

    System.Collections.IEnumerator DashDamageCoroutine(Vector2 direction)
    {
        float elapsed = 0f;

        // Durante 0.3 segundos hacemos daño a los peces cercanos
        while (elapsed < 0.3f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, dashRadius);
            foreach (Collider2D hit in hits)
            {
                FishEnemy fish = hit.GetComponent<FishEnemy>();
                if (fish != null)
                {
                    fish.TakeDamage((int)dashDamage);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
