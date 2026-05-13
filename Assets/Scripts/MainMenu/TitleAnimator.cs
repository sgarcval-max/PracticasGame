using UnityEngine;
using TMPro;

public class TitleAnimator : MonoBehaviour
{
    public float zoomDuration = 1.2f;
    public float startScale = 0f;
    public float endScale = 1f;
    public AnimationCurve zoomCurve;

    // Efecto de flotación continua
    public float floatSpeed = 1.5f;
    public float floatAmount = 0.15f;

    private float timer = 0f;
    private bool animating = true;
    private Vector3 basePosition;

    void Awake()
    {
        transform.localScale = Vector3.zero;
        basePosition = transform.position;

        // Curva de zoom con rebote
        zoomCurve = new AnimationCurve(
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.7f, 1.1f, 0f, 0f),
            new Keyframe(0.85f, 0.95f, 0f, 0f),
            new Keyframe(1f, 1f, 0f, 0f)
        );
    }

    void Update()
    {
        if (animating)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / zoomDuration);
            float scale = zoomCurve.Evaluate(t);
            transform.localScale = Vector3.one * scale;

            if (t >= 1f)
            {
                animating = false;
                basePosition = transform.position;
            }
        }
        else
        {
            // Flotación continua
            float newY = basePosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            transform.position = new Vector3(basePosition.x, newY, basePosition.z);
        }
    }
}