using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Configuración de oleadas")]
    public GameObject[] fishPrefabs;
    public int fishPerWave = 5;
    public int fishIncreaseEvery5Waves = 3;

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
        // Esperamos a que termine la transición antes de empezar
        StartCoroutine(DelayedStart());
    }

    System.Collections.IEnumerator DelayedStart()
    {
        // Esperamos a que termine la transición
        yield return new WaitUntil(() => SceneTransition.Instance == null || !SceneTransition.Instance.IsTransitioning());

        // Pequeño delay extra para que el jugador se prepare
        yield return new WaitForSeconds(1f);

        StartNextWave();
    }

    void StartNextWave()
    {
        currentWave++;
        gameUI?.UpdateWave(currentWave);

        // Avisamos al TreasureManager
        TreasureManager tm = FindFirstObjectByType<TreasureManager>();
        if (tm != null)
        {
            tm.OnWaveStart(currentWave);
        }

        // Calculamos cuantos peces salen
        // Cada 5 oleadas aumenta la cantidad
        // Oleadas 1-5 -> fishPerWave
        // Oleadas 6-10 -> fishPerWave + fishIncreaseEvery5Waves
        // Oleadas 11-15 -> fishPerWave + fishIncreaseEvery5Waves * 2
        int increments = (currentWave - 1) / 5;
        int fishCount = fishPerWave + increments * fishIncreaseEvery5Waves;

        fishAlive = fishCount;

        Debug.Log("Oleada " + currentWave + " — Peces: " + fishCount);

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

        GameObject randomFish = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
        GameObject spawnedFish = Instantiate(randomFish, spawnPos, Quaternion.identity);

        // Decidimos si este pez es domesticable desde el principio
        FishEnemy fishEnemy = spawnedFish.GetComponent<FishEnemy>();
        if (fishEnemy != null)
        {
            bool isTameable = Random.value <= fishEnemy.tameChance;
            fishEnemy.SetTameable(isTameable);
        }
    }

    public void OnFishDied()
    {
        fishAlive--;

        if (fishAlive <= 0)
        {
            // Mostramos el mensaje de oleada completada
            GameUI gameUI = FindFirstObjectByType<GameUI>();
            if (gameUI != null)
            {
                gameUI.ShowWaveComplete(currentWave, StartNextWave);
            }
            else
            {
                StartNextWave();
            }
        }
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}