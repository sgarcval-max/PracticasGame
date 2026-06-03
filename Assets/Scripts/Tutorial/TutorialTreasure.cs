using UnityEngine;
using System.Collections;

public class TutorialTreasure : MonoBehaviour
{
    private bool triggered = false;
    private bool ready = false;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => TutorialManager.Instance != null);
        ready = true;
    }

    public void OnTreasureCollected()
    {
        if (!ready || triggered) return;

        triggered = true;
        TutorialManager.Instance.TriggerAction(
                "treasure",
                "Tesoro",
                "Has recogido un fragmento de tesoro!\nNecesitas 5 para completar la misión.\nAparecen aleatoriamente en cada oleada."
        );
    }
}