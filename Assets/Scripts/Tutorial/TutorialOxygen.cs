using UnityEngine;
using System.Collections;

public class TutorialOxygen : MonoBehaviour
{
    private bool triggered = false;
    private bool ready = false;
    public float triggerThreshold = 95f;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => TutorialManager.Instance != null);
        ready = true;
    }

    void Update()
    {
        if (!ready || triggered) return;

        OxygenSystem oxygen = FindFirstObjectByType<OxygenSystem>();
        if (oxygen != null && oxygen.GetCurrentOxygen() < triggerThreshold)
        {
            triggered = true;
            TutorialManager.Instance.TriggerAction(
                "oxygen",
                "Oxígeno",
                "Tu oxígeno está bajando!\nCuando se acabe perderás vida.\nGestiona bien tu tiempo en el mar."
            );
        }
    }
}
