using UnityEngine;

public class ScreenWrapper : MonoBehaviour
{
    private Camera mainCam;
    private float camHeight;
    private float camWidth;

    void Awake()
    {
        mainCam = Camera.main;

        // Calculamos los límites de la cámara
        camHeight = mainCam.orthographicSize;
        camWidth = camHeight * mainCam.aspect;
    }

    void LateUpdate()
    {
        // Cogemos la posición actual del buzo
        Vector3 pos = transform.position;

        // Si sale por la derecha aparece por la izquierda
        if (pos.x > camWidth) pos.x = -camWidth;
        if (pos.x < -camWidth) pos.x = camWidth;

        // Si sale por arriba aparece por abajo
        if (pos.y > camHeight) pos.y = -camHeight;
        if (pos.y < -camHeight) pos.y = camHeight;

        // Aplicamos la nueva posición
        transform.position = pos;
    }
}
