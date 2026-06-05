using UnityEngine;

public class DoriAbility : FishAbility
{
    public int healAmount = 1;

    void Awake()
    {
        abilityName = "Curación";
        cooldown = 15f;
        duration = 0f;
        fishType = FishType.Cirujano;
    }

    protected override void Activate()
    {
        DiverHealth health = GetComponent<DiverHealth>();
        if (health != null)
        {
            health.Heal(healAmount);

            // Efecto visual barra de vida
            GameUI gameUI = FindFirstObjectByType<GameUI>();
            if (gameUI != null)
                gameUI.FlashHeal();

            // Efecto visual viñeta verde
            if (DamageVignette.Instance != null)
                DamageVignette.Instance.ShowHeal();

            Debug.Log("Curación! +" + healAmount + " vida");
        }
    }
}
