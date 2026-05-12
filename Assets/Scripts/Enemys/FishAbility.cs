using UnityEngine;

// Esta clase es la BASE de todas las habilidades
// Cada habilidad será un script que hereda de esta
public abstract class FishAbility : MonoBehaviour
{
    // Nombre de la habilidad
    public string abilityName = "Habilidad";

    // Segundos de espera entre usos
    public float cooldown = 5f;

    // Cuanto tiempo dura el efecto
    public float duration = 3f;

    // Tiempo hasta que se puede usar de nuevo
    private float cooldownTimer = 0f;

    void Update()
    {
        // Reducimos el timer cada frame
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    // Intentar activar la habilidad
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

    // Cada habilidad implementa su propio Activate()
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
