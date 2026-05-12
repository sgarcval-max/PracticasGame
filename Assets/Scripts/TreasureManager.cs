using UnityEngine;

public class TreasureManager : MonoBehaviour
{
    [Header("Configuración")]
    // Cuantos fragmentos hay en total
    public int totalTreasure = 5;

    // Prefab del tesoro
    public GameObject treasurePrefab;

    // Fragmentos recogidos
    private int collectedTreasure = 0;

    private Camera mainCam;
    private GameUI gameUI;

    void Awake()
    {
        mainCam = Camera.main;
        gameUI = FindFirstObjectByType<GameUI>();
    }

    void Start()
    {
        SpawnTreasures();
    }

    void SpawnTreasures()
    {
        float camH = mainCam.orthographicSize;
        float camW = camH * mainCam.aspect;

        for (int i = 0; i < totalTreasure; i++)
        {
            // Posición aleatoria dentro de la pantalla
            // con un margen para que no aparezca en los bordes
            float x = Random.Range(-camW + 1f, camW - 1f);
            float y = Random.Range(-camH + 1f, camH - 1f);

            Instantiate(treasurePrefab, new Vector3(x, y, 0), Quaternion.identity);
        }
    }

    public void CollectTreasure(int value)
    {
        collectedTreasure += value;

        gameUI?.UpdateTreasure(collectedTreasure, totalTreasure);

        if (collectedTreasure >= totalTreasure)
        {
            Debug.Log("Tesoro completo! Vuelve a la base!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.missionCompleted = true;
            }

            // Mostrar pantalla de Victoria
            gameUI?.ShowVictory();
        }
    }

    public bool IsTreasureComplete()
    {
        return collectedTreasure >= totalTreasure;
    }
}
