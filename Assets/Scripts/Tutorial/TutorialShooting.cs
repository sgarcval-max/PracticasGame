using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TutorialShooting : MonoBehaviour
{
    private bool triggered = false;
    private bool ready = false;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => TutorialManager.Instance != null);
        ready = true;
    }

    void Update()
    {
        if (!ready || triggered) return;

        // Solo detectamos el disparo si el panel NO está abierto
        if (TutorialManager.Instance.IsPanelOpen()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            triggered = true;
            TutorialManager.Instance.TriggerAction(
                "shooting",
                "Disparo",
                "Click izquierdo para disparar hacia el cursor.\nElimina a los peces enemigos para sobrevivir."
            );
        }
    }
}