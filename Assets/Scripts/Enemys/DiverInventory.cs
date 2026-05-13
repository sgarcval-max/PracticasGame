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

    void Start()
    {
        // Cargar datos del GameManager al empezar la escena
        if (GameManager.Instance != null)
        {
            allFish = new List<FishType>(GameManager.Instance.allFish);
            equippedFish = new List<FishType>(GameManager.Instance.equippedFish);

            // Añadir las habilidades de los peces equipados
            foreach (FishType fish in equippedFish)
            {
                abilityManager?.AddAbility(fish);
            }

            // Actualizar slots de la UI
            gameUI?.UpdateSlots(equippedFish.Count);

            Debug.Log("Peces cargados: " + equippedFish.Count + " equipados");
        }
    }

    public void AddFish(FishType fishType)
    {
        allFish.Add(fishType);

        if (equippedFish.Count < maxEquipped)
        {
            equippedFish.Add(fishType);

            abilityManager?.AddAbility(fishType);
            gameUI?.UpdateSlots(equippedFish.Count);

            Debug.Log("Pez equipado: " + fishType);
        }
        else
        {
            Debug.Log("Pez guardado en base: " + fishType);
        }

        // Guardar en el GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.allFish = new List<FishType>(allFish);
            GameManager.Instance.equippedFish = new List<FishType>(equippedFish);
        }
    }

    public bool HasEquipped(FishType fishType)
    {
        return equippedFish.Contains(fishType);
    }
}