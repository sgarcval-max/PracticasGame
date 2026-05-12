using UnityEngine;
using System.Collections.Generic;

public class DiverInventory : MonoBehaviour
{
    public int maxEquipped = 3;
    public List<FishType> equippedFish = new List<FishType>();
    public List<FishType> allFish = new List<FishType>();

    private AbilityManager abilityManager;
    private GameUI gameUI;

    void Awake()
    {
        abilityManager = GetComponent<AbilityManager>();
        gameUI = FindFirstObjectByType<GameUI>();
    }

    public void AddFish(FishType fishType)
    {
        allFish.Add(fishType);

        if (equippedFish.Count < maxEquipped)
        {
            equippedFish.Add(fishType);

            if (abilityManager != null)
            {
                abilityManager.AddAbility(fishType);
            }

            // Actualizamos los slots de la UI
            gameUI?.UpdateSlots(equippedFish.Count);

            Debug.Log("Pez equipado: " + fishType);
        }
        else
        {
            Debug.Log("Pez guardado en base: " + fishType);
        }
    }

    public bool HasEquipped(FishType fishType)
    {
        return equippedFish.Contains(fishType);
    }
}