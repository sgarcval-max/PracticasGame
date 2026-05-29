using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    [Header("Configuración")]
    public int totalTreasure = 5;
    public GameObject treasurePrefab;

    // Probabilidad base en la oleada 1 (10%)
    public float baseProbability = 0.1f;

    // Cuanto aumenta la probabilidad por oleada
    public float probabilityIncrease = 0.1f;

    // Tesoros recogidos
    private int collectedTreasure = 0;

    // Si ya ha salido un tesoro en esta oleada
    private bool treasureSpawnedThisWave = false;

    private Camera mainCam;
    private GameUI gameUI;

    void Awake()
    {
        mainCam = Camera.main;
        gameUI = FindFirstObjectByType<GameUI>();
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            collectedTreasure = GameManager.Instance.collectedTreasure;
            // Actualizamos el backup cada vez que entramos al mar
            GameManager.Instance.collectedTreasureBackup = GameManager.Instance.collectedTreasure;
        }

        gameUI?.UpdateTreasure(collectedTreasure, totalTreasure);
    }

    // Llamado desde WaveManager al inicio de cada oleada
    public void OnWaveStart(int waveNumber)
    {
        treasureSpawnedThisWave = false;

        // Si ya tenemos todos los tesoros no spawneamos más
        if (collectedTreasure >= totalTreasure) return;

        // Calculamos la probabilidad según la oleada
        // Oleada 1 -> 10%, Oleada 2 -> 20%, Oleada 3 -> 30%...
        float probability = baseProbability + (waveNumber - 1) * probabilityIncrease;

        // Limitamos la probabilidad al 90% máximo
        probability = Mathf.Clamp(probability, 0f, 0.9f);

        Debug.Log("Oleada " + waveNumber + " probabilidad de tesoro: " + (probability * 100) + "%");

        // Tiramos el dado
        if (Random.value <= probability)
        {
            SpawnTreasure();
        }
    }

    void SpawnTreasure()
    {
        float camH = mainCam.orthographicSize;
        float camW = camH * mainCam.aspect;

        // Posición aleatoria dentro de la pantalla con margen
        float x = Random.Range(-camW + 1f, camW - 1f);
        float y = Random.Range(-camH + 1f, camH - 1f);

        Instantiate(treasurePrefab, new Vector3(x, y, 0), Quaternion.identity);
        treasureSpawnedThisWave = true;

        Debug.Log("Tesoro spawneado en oleada!");
    }

    public void CollectTreasure(int value)
    {
        collectedTreasure += value;
        gameUI?.UpdateTreasure(collectedTreasure, totalTreasure);

        // Guardamos el progreso en el GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.collectedTreasure = collectedTreasure;

        Debug.Log("Tesoro recogido: " + collectedTreasure + "/" + totalTreasure);

        if (collectedTreasure >= totalTreasure)
        {
            Debug.Log("Tesoro completo! Aparece el boss!");
            if (GameManager.Instance != null)
                GameManager.Instance.missionCompleted = true;
            gameUI?.ShowVictory();
        }
    }

    public bool IsTreasureComplete()
    {
        return collectedTreasure >= totalTreasure;
    }
}
