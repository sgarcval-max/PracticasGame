using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TutorialShooting : MonoBehaviour
{
    private bool triggered = false;
    private bool ready = false;

    IEnumerator Start()
    {
        // Esperamos a que el TutorialManager esté listo
        yield return new WaitUntil(() => TutorialManager.Instance != null);
        ready = true;
    }

    void Update()
    {
        Debug.Log("Shooting - ready: " + ready + " triggered: " + triggered + " panelOpen: " + (TutorialManager.Instance != null ? TutorialManager.Instance.IsPanelOpen().ToString() : "NULL"));

        if (!ready || triggered) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Click detectado!");
            triggered = true;
            TutorialManager.Instance.TriggerAction(
                "shooting",
                "Disparo",
                "Click izquierdo para disparar hacia el cursor.\nElimina a los peces enemigos para sobrevivir."
            );
        }
    }
}