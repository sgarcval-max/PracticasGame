using UnityEngine;
using UnityEngine.Video;

public class MenuBackground : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public void PrepareBackground()
    {
        if (videoPlayer != null)
            videoPlayer.Prepare();
    }

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
