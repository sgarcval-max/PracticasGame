using UnityEngine;

public abstract class FishAbility : MonoBehaviour
{
    public string abilityName = "Habilidad";
    public float cooldown = 5f;
    public float duration = 3f;
    public FishType fishType;

    private float cooldownTimer = 0f;

    void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void TryActivate()
    {
        if (cooldownTimer > 0f)
        {
            Debug.Log(abilityName + " en cooldown: " + cooldownTimer.ToString("F1") + "s");
            return;
        }

        Activate();
        cooldownTimer = cooldown;
    }

    protected abstract void Activate();

    public bool IsReady()
    {
        return cooldownTimer <= 0f;
    }

    public float GetCooldownPercent()
    {
        return cooldownTimer / cooldown;
    }
}
