using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Peces capturados en el mar (sin domesticar)
    public List<FishType> caughtFish = new List<FishType>();

    // Peces domesticados (en el acuario)
    public List<FishType> tamedFish = new List<FishType>();

    // Peces equipados
    public List<FishType> equippedFish = new List<FishType>();

    // Progreso
    public int collectedTreasure = 0;
    public int currentWave = 0;
    public string currentMission = "Encuentra el cofre dorado";
    public bool missionCompleted = false;
    public bool comingFromMenu = false;

    // Copia de los peces antes de entrar al mar
    public List<FishType> caughtFishBackup = new List<FishType>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SavePlayerData(DiverInventory inventory, WaveManager waveManager, bool died = false)
    {
        // Si ha muerto usamos el backup, si no guardamos los peces actuales
        if (died)
            caughtFish = new List<FishType>(caughtFishBackup);
        else
            caughtFish = new List<FishType>(inventory.caughtFish);

        tamedFish = new List<FishType>(inventory.tamedFish);
        equippedFish = new List<FishType>(inventory.equippedFish);
        currentWave = waveManager != null ? waveManager.GetCurrentWave() : 0;
    }
}