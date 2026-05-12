using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Configuración de oleadas")]
    public GameObject[] fishPrefabs; // Array con todos los prefabs de peces
    public int fishPerWave = 5;
    public int fishIncreasePerWave = 2;

    private int currentWave = 0;
    private int fishAlive = 0;
    private Camera mainCam;
    private GameUI gameUI;

    void Awake()
    {
        mainCam = Camera.main;
        gameUI = FindFirstObjectByType<GameUI>();
    }

    void Start()
    {
        StartNextWave();
    }

    void StartNextWave()
    {
        currentWave++;
        gameUI?.UpdateWave(currentWave);

        int fishCount = fishPerWave + (currentWave - 1) * fishIncreasePerWave;
        fishAlive = fishCount;

        for (int i = 0; i < fishCount; i++)
        {
            SpawnFish();
        }
    }

    void SpawnFish()
    {
        int side = Random.Range(0, 4);

        float camH = mainCam.orthographicSize;
        float camW = camH * mainCam.aspect;

        Vector2 spawnPos = Vector2.zero;

        switch (side)
        {
            case 0:
                spawnPos = new Vector2(Random.Range(-camW, camW), camH + 0.5f);
                break;
            case 1:
                spawnPos = new Vector2(Random.Range(-camW, camW), -camH - 0.5f);
                break;
            case 2:
                spawnPos = new Vector2(-camW - 0.5f, Random.Range(-camH, camH));
                break;
            case 3:
                spawnPos = new Vector2(camW + 0.5f, Random.Range(-camH, camH));
                break;
        }

        // Elegimos un prefab aleatorio del array
        GameObject randomFish = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
        Instantiate(randomFish, spawnPos, Quaternion.identity);
    }

    public void OnFishDied()
    {
        fishAlive--;

        if (fishAlive <= 0)
        {
            StartNextWave();
        }
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}