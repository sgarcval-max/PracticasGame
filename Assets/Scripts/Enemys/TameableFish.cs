using UnityEditor.Build.Content;
using UnityEngine;

public class TameableFish : MonoBehaviour
{
    // Distancia a la que el buzo puede recogerlo
    public float pickupRadius = 1f;

    // Tiempo antes de que desaparezca si no lo recoges
    public float lifetime = 8f;

    // Tipo de pez (lo usaremos luego para las habilidades)
    public FishType fishType;

    private Transform player;
    private SpriteRenderer sr;
    private float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.Find("Diver").transform;
        timer = lifetime;
    }

    void Start()
    {
        // Ahora fishType ya está asignado
        SetFishSprite();
    }

    void SetFishSprite()
    {
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager == null)
        {
            Debug.Log("WaveManager no encontrado");
            return;
        }

        Debug.Log("Buscando sprite para: " + fishType);

        foreach (GameObject prefab in waveManager.fishPrefabs)
        {
            FishEnemy fe = prefab.GetComponent<FishEnemy>();
            if (fe != null)
            {
                Debug.Log("Prefab encontrado: " + fe.fishType);
                if (fe.fishType == fishType)
                {
                    SpriteRenderer prefabSr = prefab.GetComponent<SpriteRenderer>();
                    if (prefabSr != null && prefabSr.sprite != null)
                    {
                        Debug.Log("Sprite asignado: " + prefabSr.sprite.name);
                        sr.sprite = prefabSr.sprite;
                        sr.color = Color.white;
                        StartCoroutine(GlowEffect());
                    }
                    transform.localScale = prefab.transform.localScale;
                    return;
                }
            }
        }
    }

    System.Collections.IEnumerator GlowEffect()
    {
        while (true)
        {
            float t = Mathf.Sin(Time.time * 3f) * 0.5f + 0.5f;
            sr.color = Color.Lerp(Color.white, new Color(1f, 1f, 0.5f), t);
            yield return null;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Parpadea cuando le queda poco tiempo
        timer -= Time.deltaTime;
        if (timer <= 3f)
        {
            sr.enabled = Mathf.Sin(timer * 10f) > 0;
        }

        // Si se acaba el tiempo desaparece
        if (timer <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        // Si el buzo se acerca lo suficiente lo recoge
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= pickupRadius)
        {
            Collect();
        }
    }

    void Collect()
    {
        DiverInventory inventory = player.GetComponent<DiverInventory>();
        if (inventory != null)
        {
            // Ahora va a la mochila, no al inventario directamente
            inventory.CatchFish(fishType);
        }

        Destroy(gameObject);
    }

    // Dibuja el radio de recogida en el editor para verlo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
