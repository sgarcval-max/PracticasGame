using UnityEngine;
using System.Collections;

public class TutorialFishWave : MonoBehaviour
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

        FishEnemy[] fish = FindObjectsByType<FishEnemy>(FindObjectsSortMode.None);
        if (fish.Length > 0)
        {
            triggered = true;
            TutorialManager.Instance.TriggerAction(
                "fishwave",
                "Oleada de Peces",
                "Los peces vienen a por ti!\nElimínalos todos para pasar a la siguiente oleada.\nCada 5 oleadas aparecen más peces."
            );
        }
    }
}
