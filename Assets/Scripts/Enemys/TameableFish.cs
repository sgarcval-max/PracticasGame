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
