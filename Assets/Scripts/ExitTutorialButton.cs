using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTutorialButton : MonoBehaviour
{
    // Asegúrate de que este nombre sea EXACTAMENTE igual al de tu escena de menú en la carpeta Scenes
    public string nombreDelMenu = "MenuPrincipal";

    public void VolverAlMenu()
    {
        Debug.Log("Botón pulsado: Iniciando salida del tutorial...");

        // 1. OBLIGATORIO: Descongelar el tiempo del juego antes de cambiar de escena
        Time.timeScale = 1f;

        // 2. Guardamos que el tutorial general ya se ha completado
        PlayerPrefs.SetInt("TutorialCompletado", 1);
        PlayerPrefs.Save();

        // 3. Cargamos la escena del menú principal
        SceneManager.LoadScene(nombreDelMenu);
    }
}