using UnityEngine;
using System.Collections.Generic;

public class DiverInventory : MonoBehaviour
{
    public int maxEquipped = 3;
    public List<FishType> caughtFish = new List<FishType>();
    public List<FishType> tamedFish = new List<FishType>();
    public List<FishType> equippedFish = new List<FishType>();

    private AbilityManager abilityManager;
    private GameUI gameUI;

    void Awake()
    {
        abilityManager = GetComponent<AbilityManager>();
        gameUI = FindFirstObjectByType<GameUI>();
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            equippedFish = new List<FishType>(GameManager.Instance.equippedFish);
            tamedFish = new List<FishType>(GameManager.Instance.tamedFish);
            caughtFish = new List<FishType>(GameManager.Instance.caughtFish);

            // Guardamos backup al entrar al mar
            GameManager.Instance.caughtFishBackup = new List<FishType>(GameManager.Instance.caughtFish);

            foreach (FishType fish in equippedFish)
                abilityManager?.AddAbility(fish);

            gameUI?.UpdateSlots(equippedFish.Count);
        }
    }

    public void CatchFish(FishType fishType)
    {
        caughtFish.Add(fishType);
        Debug.Log("Pez capturado en mochila: " + fishType);

        if (GameManager.Instance != null)
            GameManager.Instance.caughtFish = new List<FishType>(caughtFish);
    }

    public void TameFish(FishType fishType)
    {
        if (caughtFish.Contains(fishType))
        {
            caughtFish.Remove(fishType);
            tamedFish.Add(fishType);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.caughtFish = new List<FishType>(caughtFish);
                GameManager.Instance.tamedFish = new List<FishType>(tamedFish);
            }

            Debug.Log("Pez domesticado: " + fishType);
        }
    }

    public void LoseFish(FishType fishType)
    {
        if (caughtFish.Contains(fishType))
        {
            caughtFish.Remove(fishType);

            if (GameManager.Instance != null)
                GameManager.Instance.caughtFish = new List<FishType>(caughtFish);

            Debug.Log("Pez perdido: " + fishType);
        }
    }

    public void EquipFish(FishType fishType)
    {
        if (equippedFish.Count >= maxEquipped) return;
        if (equippedFish.Contains(fishType)) return;
        if (!tamedFish.Contains(fishType)) return;

        equippedFish.Add(fishType);
        abilityManager?.AddAbility(fishType);
        gameUI?.UpdateSlots(equippedFish.Count);

        if (GameManager.Instance != null)
            GameManager.Instance.equippedFish = new List<FishType>(equippedFish);
    }

    public bool HasEquipped(FishType fishType)
    {
        return equippedFish.Contains(fishType);
    }
}