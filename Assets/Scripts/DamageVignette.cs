using UnityEngine;
using UnityEngine.UI;

public class DamageVignette : MonoBehaviour
{
    public static DamageVignette Instance;

    public Image vignetteImage;
    public float fadeSpeed = 3f;

    private float targetAlpha = 0f;
    private Color vignetteColor;

    void Awake()
    {
        Instance = this;
        vignetteColor = new Color(1f, 0f, 0f, 0f);
        if (vignetteImage != null)
            vignetteImage.color = vignetteColor;
    }

    void Update()
    {
        if (vignetteImage == null) return;

        // Fade hacia el target alpha
        vignetteColor.a = Mathf.MoveTowards(vignetteColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
        vignetteImage.color = vignetteColor;

        // Una vez mostrado volvemos a 0
        if (vignetteColor.a >= targetAlpha && targetAlpha > 0f)
            targetAlpha = 0f;
    }

    public void ShowDamage()
    {
        targetAlpha = 0f;
        vignetteColor.a = 0.6f;
        if (vignetteImage != null)
            vignetteImage.color = vignetteColor;
    }
}
