using UnityEngine;
using System.Collections;

public class TutorialDamage : MonoBehaviour
{
    private bool triggered = false;
    private bool ready = false;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => TutorialManager.Instance != null);
        ready = true;
    }

    public void OnDamageReceived()
    {
        if (!ready || triggered) return;

        triggered = true;
        TutorialManager.Instance.TriggerAction(
            "damage",
            "Vida",
            "Has recibido daño!\nTu barra de vida está arriba a la izquierda.\nSi llega a 0 pierdes."
        );
    }
}
