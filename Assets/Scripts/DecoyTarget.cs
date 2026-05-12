using UnityEngine;

public class DecoyTarget : MonoBehaviour
{
    void OnEnable()
    {
        // Avisamos a todos los peces que hay un señuelo
        FishEnemy[] allFish = FindObjectsByType<FishEnemy>(FindObjectsSortMode.None);
        foreach (FishEnemy fish in allFish)
        {
            fish.SetTarget(transform);
        }
    }

    void OnDestroy()
    {
        // Cuando se destruye los peces vuelven a perseguir al buzo
        FishEnemy[] allFish = FindObjectsByType<FishEnemy>(FindObjectsSortMode.None);
        GameObject player = GameObject.Find("Diver");
        if (player == null) return;

        foreach (FishEnemy fish in allFish)
        {
            fish.SetTarget(player.transform);
        }
    }
}