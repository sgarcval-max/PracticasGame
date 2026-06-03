using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingController : MonoBehaviour
{
    // Arrastramos aquí el Prefab de la bala
    public GameObject projectilePrefab;

    // Arrastramos aquí el FirePoint
    public Transform firePoint;

    // Tiempo entre disparos
    public float fireRate = 0.3f;

    private float nextFireTime = 0f;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // Apuntar hacia el cursor siempre
        AimAtMouse();

        // Si hacemos click izquierdo y ha pasado suficiente tiempo
        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void AimAtMouse()
    {
        // Convertir posición del cursor a coordenadas del mundo
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0f;

        // Calcular dirección desde el buzo al cursor
        Vector2 direction = (mousePos - transform.position).normalized;

        // Rotar el FirePoint hacia esa dirección
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Crear una bala en la posición y rotación del FirePoint
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        FindObjectOfType<TutorialPopUpManager>()?.MostrarTutorial("Disparo", "Clic Izquierdo para disparar a los enemigos.");
        TutorialManager.Instance?.CompletarObjetivo("Disparo");
    }
}