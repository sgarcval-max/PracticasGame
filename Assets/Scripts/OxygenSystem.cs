using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OxygenSystem : MonoBehaviour
{
    [Header("Configuración")]
    public float maxOxygen = 100f;
    public float oxygenDuration = 120f;
    public float damagePerSecond = 1f;
    public float damageCooldown = 1f;

    [Header("UI")]
    public Slider oxygenBar;
    public TextMeshProUGUI oxygenText;

    [Header("Objeto que tiembla")]
    public RectTransform oxygenUIObject;
    public float shakeAmount = 5f;
    public float shakeSpeed = 20f;

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
    private Vector3 originalUIPosition;

    void Awake()
    {
        diverHealth = GetComponent<DiverHealth>();
        gameUI = FindFirstObjectByType<GameUI>();

        oxygenDecreaseRate = maxOxygen / oxygenDuration;
        currentOxygen = maxOxygen;

        if (oxygenBar != null)
        {
            fillImage = oxygenBar.fillRect.GetComponent<Image>();
            oxygenBar.maxValue = maxOxygen;
            oxygenBar.value = maxOxygen;
        }

        // Guardamos posición original del objeto UI
        if (oxygenUIObject != null)
            originalUIPosition = oxygenUIObject.localPosition;
    }

    void Update()
    {
        if (currentOxygen > 0f)
        {
            currentOxygen -= oxygenDecreaseRate * Time.deltaTime;
            currentOxygen = Mathf.Max(currentOxygen, 0f);

            UpdateUI();

            if (fillImage != null)
            {
                float t = currentOxygen / maxOxygen;
                fillImage.color = Color.Lerp(lowColor, fullColor, t);
            }

            // Temblor suave cuando queda poco oxígeno
            if (currentOxygen <= lowOxygenThreshold && oxygenUIObject != null)
            {
                float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
                float shakeY = Mathf.Cos(Time.time * shakeSpeed * 1.3f) * shakeAmount;
                oxygenUIObject.localPosition = originalUIPosition + new Vector3(shakeX, shakeY, 0f);
            }
            else if (oxygenUIObject != null)
            {
                oxygenUIObject.localPosition = originalUIPosition;
            }

            isOutOfOxygen = false;
        }
        else
        {
            // Temblor más fuerte sin oxígeno
            if (oxygenUIObject != null)
            {
                float shakeX = Mathf.Sin(Time.time * shakeSpeed * 2f) * shakeAmount * 2f;
                float shakeY = Mathf.Cos(Time.time * shakeSpeed * 2.5f) * shakeAmount * 2f;
                oxygenUIObject.localPosition = originalUIPosition + new Vector3(shakeX, shakeY, 0f);
            }

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

                if (DamageVignette.Instance != null)
                    DamageVignette.Instance.ShowDamage();
            }
        }
    }

    void UpdateUI()
    {
        if (oxygenBar != null)
            oxygenBar.value = currentOxygen;

        if (oxygenText != null)
        {
            if (currentOxygen <= 0f)
                oxygenText.text = "SIN o2!";
            else
                oxygenText.text = "o2";

            if (currentOxygen <= lowOxygenThreshold)
                oxygenText.color = Color.Lerp(Color.red, Color.white, Mathf.Sin(Time.time * 5f) * 0.5f + 0.5f);
            else
                oxygenText.color = new Color(0.4f, 0.8f, 1f);
        }
    }

    public void RefillOxygen()
    {
        currentOxygen = maxOxygen;
        isOutOfOxygen = false;
        if (oxygenUIObject != null)
            oxygenUIObject.localPosition = originalUIPosition;
    }
}