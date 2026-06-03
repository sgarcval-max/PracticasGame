using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Configuración de Objetivos")]
    public int objetivosTotales = 4; // Ej: 1.Mover, 2.Disparar, 3.Recibir Daño, 4.Recoger Pez
    private int objetivosCompletados = 0;

    [Header("UI")]
    public GameObject botonFinalizarTutorial; // El botón que empieza oculto

    void Awake()
    {
        Instance = this;
        if (botonFinalizarTutorial != null)
            botonFinalizarTutorial.SetActive(false); // Ocultar al empezar
    }

    // Esta función la llamaremos desde los otros scripts
    public void CompletarObjetivo(string id)
    {
        // Usamos PlayerPrefs temporales para saber si YA completó este paso en esta sesión
        if (PlayerPrefs.GetInt("Temp_" + id, 0) == 0)
        {
            objetivosCompletados++;
            PlayerPrefs.SetInt("Temp_" + id, 1);
            Debug.Log("Objetivo completado: " + id + ". Total: " + objetivosCompletados + "/" + objetivosTotales);

            if (objetivosCompletados >= objetivosTotales)
            {
                ActivarBotonSalida();
            }
        }
    }

    void ActivarBotonSalida()
    {
        if (botonFinalizarTutorial != null)
        {
            botonFinalizarTutorial.SetActive(true);
            Debug.Log("¡Tutorial completado al 100%! Botón activado.");
        }
    }

    // Limpiar datos temporales al salir
    void OnDestroy()
    {
        PlayerPrefs.DeleteKey("Temp_Movimiento");
        PlayerPrefs.DeleteKey("Temp_Disparo");
        PlayerPrefs.DeleteKey("Temp_Dano");
        PlayerPrefs.DeleteKey("Temp_Captura");
    }
}
