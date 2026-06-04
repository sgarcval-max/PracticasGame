using UnityEngine;
using UnityEngine.Video;

public class MenuBackground : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    // Llamar este método cuando el menú aparece
    public void StartBackground()
    {
        if (videoPlayer != null)
            videoPlayer.Play();
    }

    public void StopBackground()
    {
        if (videoPlayer != null)
            videoPlayer.Stop();
    }
}
