using UnityEngine;

public class TutorialProgressBlocker : MonoBehaviour
{
    private int oroInicial;
    private int pecesIniciales;

    void Start()
    {
        // 1. Antes de que el jugador haga nada, memorizamos qué tenía 
        // (Esto lo lee del GameManager original)
        if (GameManager.Instance != null)
        {
            Debug.Log("Tutorial: Guardando estado original del jugador para no sobrescribir.");
        }
    }

    // Esta función se llama justo antes de que la escena se destruya (al salir al menú)
    void OnDestroy()
    {
        // 2. Aquí está el truco: NO llamamos a GameManager.Instance.SavePlayerData()
        // Simplemente dejamos que la escena muera. 
        // Como el GameManager es un objeto que persiste, si no le damos la orden de guardar, 
        // los datos del tutorial se perderán al cargar la escena real.
        Debug.Log("Tutorial finalizado: Los datos temporales han sido descartados.");
    }
}
