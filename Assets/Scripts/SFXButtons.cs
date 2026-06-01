using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SFXButtons : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        // Añadimos el sonido de click también desde el onClick del botón
        if (button != null)
        {
            button.onClick.AddListener(() =>
            {
                if (AudioManager.Instance != null && AudioManager.Instance.buttonClickSound != null)
                    AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
            });
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter llamado. AudioManager: " + (AudioManager.Instance != null ? "OK" : "NULL"));
        if (AudioManager.Instance != null && AudioManager.Instance.buttonHoverSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonHoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.buttonClickSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
    }
}
