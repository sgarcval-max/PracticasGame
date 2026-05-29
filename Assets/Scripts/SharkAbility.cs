using UnityEngine;

public class SharkAbility : FishAbility
{
    public float shieldDuration = 4f;

    private DiverHealth diverHealth;
    private SpriteRenderer sr;
    private Color originalColor;

    void Awake()
    {
        abilityName = "Escudo";
        cooldown = 12f;
        duration = shieldDuration;
        fishType = FishType.Shark;

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
        diverHealth.SetInvincible(true);
        sr.color = new Color(0.2f, 0.5f, 1f);

        yield return new WaitForSeconds(shieldDuration);

        diverHealth.SetInvincible(false);
        sr.color = originalColor;
        Debug.Log("Escudo terminado!");
    }
}
