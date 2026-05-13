using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float spawnRate = 0.5f;
    private float timer = 0f;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            timer = 0f;
            SpawnBubble();
        }
    }

    void SpawnBubble()
    {
        float camH = mainCam.orthographicSize;
        float camW = camH * mainCam.aspect;

        // Spawnea en la parte de abajo en posición aleatoria
        Vector3 pos = new Vector3(
            Random.Range(-camW, camW),
            -camH - 0.5f,
            0f
        );

        Instantiate(bubblePrefab, pos, Quaternion.identity);
    }
}