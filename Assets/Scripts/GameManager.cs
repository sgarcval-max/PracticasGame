using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Instancia única del GameManager
    public static GameManager Instance;

    // Datos del jugador que se guardan entre escenas
    public List<FishType> allFish = new List<FishType>();
    public List<FishType> equippedFish = new List<FishType>();
    public int currentWave = 0;

    // Misión actual
    public string currentMission = "Encuentra el cofre dorado";
    public bool missionCompleted = false;

    void Awake()
    {
        // Si ya existe un GameManager no creamos otro
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Este objeto no se destruye al cambiar de escena
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SavePlayerData(DiverInventory inventory, WaveManager waveManager)
    {
        allFish = new List<FishType>(inventory.allFish);
        equippedFish = new List<FishType>(inventory.equippedFish);
        currentWave = waveManager != null ? waveManager.GetCurrentWave() : 0;
    }
}