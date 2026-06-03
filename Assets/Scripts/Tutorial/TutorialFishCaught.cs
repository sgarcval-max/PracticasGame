using UnityEngine;
using System.Collections;

public class TutorialFishCaught : MonoBehaviour
{
    private bool triggered = false;
    private bool ready = false;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => TutorialManager.Instance != null);
        ready = true;
    }

    public void OnFishCaught()
    {
        if (!ready || triggered) return;

        triggered = true;
        TutorialManager.Instance.TriggerAction(
            "fishcaught",
            "Pez Capturado",
            "Has capturado un pez!\nLos peces dorados pueden ser capturados.\nVuelve a la base para domesticarlos y obtener habilidades."
        );
    }
}
