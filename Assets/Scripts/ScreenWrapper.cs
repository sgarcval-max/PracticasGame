using UnityEngine;

public class ScreenWrapper : MonoBehaviour
{
    [Header("Modo")]
    public bool hardBorder = false; // false = wrap, true = límite duro

    private Camera mainCam;
    private float camHeight;
    private float camWidth;
    private Rigidbody2D rb;

    void Awake()
    {
        mainCam = Camera.main;
        camHeight = mainCam.orthographicSize;
        camWidth = camHeight * mainCam.aspect;
        rb = GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        if (hardBorder)
        {
            // Límite duro, el player no puede salir
            pos.x = Mathf.Clamp(pos.x, -camWidth, camWidth);
            pos.y = Mathf.Clamp(pos.y, -camHeight, camHeight);

            // Si choca con el borde paramos la velocidad en esa dirección
            if (rb != null)
            {
                Vector2 vel = rb.linearVelocity;
                if (pos.x <= -camWidth || pos.x >= camWidth) vel.x = 0f;
                if (pos.y <= -camHeight || pos.y >= camHeight) vel.y = 0f;
                rb.linearVelocity = vel;
            }
        }
        else
        {
            // Wrap estilo Asteroids
            if (pos.x > camWidth) pos.x = -camWidth;
            if (pos.x < -camWidth) pos.x = camWidth;
            if (pos.y > camHeight) pos.y = -camHeight;
            if (pos.y < -camHeight) pos.y = camHeight;
        }

        transform.position = pos;
    }
}
