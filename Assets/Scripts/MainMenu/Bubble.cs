using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float speed = 1.5f;
    public float wobbleSpeed = 2f;
    public float wobbleAmount = 0.3f;

    private float startX;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
        startX = transform.position.x;

        // Tamaño aleatorio
        float size = Random.Range(0.05f, 0.25f);
        transform.localScale = new Vector3(size, size, 1f);

        // Transparencia aleatoria
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Random.Range(0.2f, 0.6f);
            sr.color = c;
        }
    }

    void Update()
    {
        // Sube hacia arriba
        transform.position += Vector3.up * speed * Time.deltaTime;

        // Movimiento ondulante
        float newX = startX + Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        transform.position = new Vector3(newX, transform.position.y, 0f);

        // Se destruye cuando sale por arriba
        float camH = mainCam.orthographicSize;
        if (transform.position.y > camH + 1f)
        {
            Destroy(gameObject);
        }
    }
}