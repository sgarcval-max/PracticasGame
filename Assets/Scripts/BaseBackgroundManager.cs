using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class BaseBackgroundManager : MonoBehaviour
{
    [Header("Componentes de Vídeo")]
    public VideoPlayer videoPlayer;
    public RawImage fondoRawImage;

    [Header("Configuración Opcional")]
    public bool empezarEnNegro = true; // Evita que se vea un cuadro blanco mientras carga

    void Start()
    {
        // Iniciamos el proceso de preparación automática
        if (videoPlayer != null && fondoRawImage != null)
        {
            StartCoroutine(PrepararYReproducirFondo());
        }
        else
        {
            Debug.LogError("BaseBackgroundManager: ¡Faltan referencias por arrastrar en el Inspector!");
        }
    }

    IEnumerator PrepararYReproducirFondo()
    {
        // 1. Forzamos que el vídeo esté en bucle continuo
        videoPlayer.isLooping = true;

        // 2. Si queremos evitar el "flash" blanco inicial de la RawImage:
        if (empezarEnNegro)
        {
            fondoRawImage.color = Color.black;
        }

        // 3. Le decimos al VideoPlayer que cargue el archivo en memoria (Buffer)
        videoPlayer.Prepare();

        // 4. Esperamos los frames que haga falta hasta que esté 100% listo
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        // 5. El vídeo ya está listo en memoria, le damos al Play
        videoPlayer.Play();

        // 6. Esperamos un frame técnico para que la tarjeta gráfica pinte el primer frame del vídeo
        yield return new WaitForEndOfFrame();

        // 7. Devolvemos el color blanco a la RawImage para que el vídeo se vea con sus colores reales
        fondoRawImage.color = Color.white;

        Debug.Log("BaseBackgroundManager: Fondo de la base reproduciéndose en bucle con éxito.");
    }

    // Por si necesitas pausarlo desde algún menú de pausa dentro de la base en el futuro
    public void PausarFondo(bool pausar)
    {
        if (videoPlayer == null) return;

        if (pausar)
            videoPlayer.Pause();
        else
            videoPlayer.Play();
    }
}
