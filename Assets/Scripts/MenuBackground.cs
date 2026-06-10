using UnityEngine;
using UnityEngine.Video;

public class MenuBackground : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Awake()
    {
        if (videoPlayer != null)
        {
            videoPlayer.isLooping = true;
            videoPlayer.playOnAwake = false;
            // Lo dejamos preparado en memoria desde el segundo uno
            videoPlayer.Prepare();
        }
    }

    // Un único método para arrancar el fondo de forma segura
    public void StartBackground()
    {
        if (videoPlayer == null) return;

        videoPlayer.isLooping = true;

        // Si ya está preparado, le damos Play de golpe
        if (videoPlayer.isPrepared)
        {
            videoPlayer.Play();
            Debug.Log("MenuBackground: ¡Vídeo reproducido al instante!");
        }
        else
        {
            // Si por lo que sea no estaba listo, nos suscribimos a su evento nativo
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        source.prepareCompleted -= OnVideoPrepared; // Nos desuscribimos por seguridad
        source.Play();
        Debug.Log("MenuBackground: Vídeo reproducido tras preparación tardía.");
    }

    public void StopBackground()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
    }
}
