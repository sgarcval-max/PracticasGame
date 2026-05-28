using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OxygenSystem : MonoBehaviour
{
    [Header("Configuración")]
    public float maxOxygen = 100f;
    public float oxygenDuration = 120f; // Segundos que dura el oxígeno
    public float damagePerSecond = 1f;  // Daño por segundo sin oxígeno
    public float damageCooldown = 1f;   // Cada cuántos segundos hace daño

    [Header("UI")]
    public Slider oxygenBar;
    public TextMeshProUGUI oxygenText;

    [Header("Colores")]
    public Color fullColor = new Color(0.4f, 0.8f, 1f);
    public Color lowColor = new Color(1f, 0.3f, 0.3f);
    public float lowOxygenThreshold = 30f;

    private float currentOxygen;
    private float oxygenDecreaseRate;
    private float damageTimer = 0f;
    private bool isOutOfOxygen = false;

    private DiverHealth diverHealth;
    private GameUI gameUI;
    private Image fillImage;

    void Awake()
    {
        diverHealth = FindFirstObjectByType<DiverHealth>();
        gameUI = FindFirstObjectByType<GameUI>();

        // Calculamos cuánto oxígeno se pierde por segundo
        oxygenDecreaseRate = maxOxygen / oxygenDuration;

        currentOxygen = maxOxygen;

        // Guardamos referencia al fill del slider
        if (oxygenBar != null)
        {
            fillImage = oxygenBar.fillRect.GetComponent<Image>();
            oxygenBar.maxValue = maxOxygen;
            oxygenBar.value = maxOxygen;
        }
    }

    void Update()
    {
        if (currentOxygen > 0f)
        {
            // Reducimos el oxígeno con el tiempo
            currentOxygen -= oxygenDecreaseRate * Time.deltaTime;
            currentOxygen = Mathf.Max(currentOxygen, 0f);

            UpdateUI();

            // Cambiar color según nivel
            if (fillImage != null)
            {
                float t = currentOxygen / maxOxygen;
                fillImage.color = Color.Lerp(lowColor, fullColor, t);
            }

            isOutOfOxygen = false;
        }
        else
        {
            // Sin oxígeno hacemos daño al buzo
            if (!isOutOfOxygen)
            {
                isOutOfOxygen = true;
                Debug.Log("Sin oxígeno!");
            }

            damageTimer += Time.deltaTime;
            if (damageTimer >= damageCooldown)
            {
                damageTimer = 0f;
                if (diverHealth != null)
                    diverHealth.TakeDamage((int)damagePerSecond);
            }
        }
    }

    void UpdateUI()
    {
        if (oxygenBar != null)
            oxygenBar.value = currentOxygen;

        if (oxygenText != null)
        {
            int seconds = Mathf.CeilToInt(currentOxygen / oxygenDecreaseRate);
            if (currentOxygen <= 0f)
                oxygenText.text = "SIN o2!";
            else
                oxygenText.text = "o2";

            // Parpadeo cuando queda poco
            if (currentOxygen <= lowOxygenThreshold)
                oxygenText.color = Color.Lerp(Color.red, Color.white, Mathf.Sin(Time.time * 5f) * 0.5f + 0.5f);
            else
                oxygenText.color = new Color(0.4f, 0.8f, 1f);
        }
    }

    // Llamar esto para recargar el oxígeno
    public void RefillOxygen()
    {
        currentOxygen = maxOxygen;
        isOutOfOxygen = false;
    }
}