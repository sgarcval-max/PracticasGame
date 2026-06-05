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

        vignetteColor.a = Mathf.MoveTowards(vignetteColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
        vignetteImage.color = vignetteColor;

        if (vignetteColor.a >= targetAlpha && targetAlpha > 0f)
            targetAlpha = 0f;
    }

    public void ShowDamage()
    {
        targetAlpha = 0f;
        vignetteColor = new Color(1f, 0f, 0f, 0.6f);
        if (vignetteImage != null)
            vignetteImage.color = vignetteColor;
    }

    public void ShowHeal()
    {
        targetAlpha = 0f;
        vignetteColor = new Color(0f, 1f, 0.3f, 0.6f);
        if (vignetteImage != null)
            vignetteImage.color = vignetteColor;
    }
}
