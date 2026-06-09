using UnityEngine;
using System.Collections;

public class SquidAbility : FishAbility
{
    public float slowDuration = 4f;
    public float slowRadius = 4f;
    public float slowMultiplier = 0.3f;
    public GameObject slowEffectPrefab;

    void Awake()
    {
        abilityName = "Nube de tinta";
        cooldown = 10f;
        duration = slowDuration;
        fishType = FishType.Squid;
    }

    protected override void Activate()
    {
        Debug.Log("Nube de tinta!");
        StartCoroutine(InkCoroutine());
    }

    IEnumerator InkCoroutine()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, slowRadius);

        // Lista de peces afectados y sus efectos
        System.Collections.Generic.List<FishEnemy> affectedFish = new System.Collections.Generic.List<FishEnemy>();
        System.Collections.Generic.List<GameObject> spawnedEffects = new System.Collections.Generic.List<GameObject>();

        foreach (Collider2D hit in hits)
        {
            FishEnemy fish = hit.GetComponent<FishEnemy>();
            if (fish != null)
            {
                fish.SetSpeed(fish.speed * slowMultiplier);
                affectedFish.Add(fish);

                // Spawneamos el efecto encima del pez
                if (slowEffectPrefab != null)
                {
                    GameObject effect = Instantiate(slowEffectPrefab, fish.transform.position, Quaternion.identity);
                    effect.transform.SetParent(fish.transform);
                    effect.transform.localPosition = new Vector3(0f, 0.3f, 0f);
                    spawnedEffects.Add(effect);
                }
            }
        }

        yield return new WaitForSeconds(slowDuration);

        // Restauramos velocidad y ocultamos efectos
        FishEnemy[] allFish = FindObjectsByType<FishEnemy>(FindObjectsSortMode.None);
        foreach (FishEnemy fish in allFish)
            fish.RestoreSpeed();

        foreach (GameObject effect in spawnedEffects)
        {
            if (effect != null)
                Destroy(effect);
        }

        Debug.Log("Tinta terminada!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, slowRadius);
    }
}