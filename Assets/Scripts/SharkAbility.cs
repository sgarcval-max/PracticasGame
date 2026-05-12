using UnityEngine;

public class SharkAbility : FishAbility
{
    // Cuanto dura el escudo
    public float shieldDuration = 4f;

    private DiverHealth diverHealth;
    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        abilityName = "Escudo";
        cooldown = 12f;
        duration = shieldDuration;

        diverHealth = GetComponent<DiverHealth>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    protected override void Activate()
    {
        Debug.Log("Escudo activado!");
        StartCoroutine(ShieldCoroutine());
    }

    System.Collections.IEnumerator ShieldCoroutine()
    {
        // Activamos invencibilidad
        diverHealth.SetInvincible(true);

        // Ponemos el buzo azul para indicar escudo
        sr.color = new Color(0.2f, 0.5f, 1f);

        // Esperamos la duración del escudo
        yield return new WaitForSeconds(shieldDuration);

        // Quitamos invencibilidad
        diverHealth.SetInvincible(false);
        sr.color = originalColor;

        Debug.Log("Escudo terminado!");
    }
}
