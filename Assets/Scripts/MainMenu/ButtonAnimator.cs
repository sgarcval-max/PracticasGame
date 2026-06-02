using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public float hoverScale = 1.1f;
    public float animSpeed = 8f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void OnDisable()
    {
        // SEGURO 1: Si el botón se desactiva, reseteamos la escala al instante
        // Esto evita que al volver a abrir el panel el botón aparezca grande.
        transform.localScale = originalScale;
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.unscaledDeltaTime * animSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // SEGURO 2: Al hacer clic, forzamos que el objetivo sea la escala original.
        // Esto soluciona el bug de que se quede "grande" al abrir/cerrar menús.
        targetScale = originalScale;
    }
}