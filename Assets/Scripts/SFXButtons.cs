using UnityEngine;
using UnityEngine.EventSystems;

public class SFXButtons : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.buttonHoverSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonHoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.buttonClickSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
    }
}
