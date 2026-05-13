using UnityEngine;

public class SquidAbility : FishAbility
{
    public float slowDuration = 4f;
    public float slowRadius = 4f;
    public float slowMultiplier = 0.3f;

    void Awake()
    {
        abilityName = "Nube de tinta";
        cooldown = 10f;
        duration = slowDuration;
        fishType = FishType.Pufferfish; // <- añade esta línea
    }

    protected override void Activate()
    {
        Debug.Log("Nube de tinta!");
        StartCoroutine(InkCoroutine());
    }

    System.Collections.IEnumerator InkCoroutine()
    {
        // Buscamos todos los peces en el radio
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, slowRadius);

        foreach (Collider2D hit in hits)
        {
            FishEnemy fish = hit.GetComponent<FishEnemy>();
            if (fish != null)
            {
                // Ralentizamos el pez
                fish.SetSpeed(fish.speed * slowMultiplier);
            }
        }

        yield return new WaitForSeconds(slowDuration);

        // Restauramos la velocidad de los peces
        FishEnemy[] allFish = FindObjectsByType<FishEnemy>(FindObjectsSortMode.None);
        foreach (FishEnemy fish in allFish)
        {
            fish.RestoreSpeed();
        }

        Debug.Log("Tinta terminada!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, slowRadius);
    }
}