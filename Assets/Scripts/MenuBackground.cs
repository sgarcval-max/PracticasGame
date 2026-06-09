using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class MenuBackground : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Awake()
    {
        // Preparamos el video desde el principio
        if (videoPlayer != null)
        {
            videoPlayer.Prepare();
            videoPlayer.playOnAwake = false;
        }
    }

    public void PrepareBackground()
    {
        if (videoPlayer != null)
            videoPlayer.Prepare();
    }

    public void StartBackground()
    {
        if (videoPlayer != null)
            StartCoroutine(WaitAndPlay());
    }

    IEnumerator WaitAndPlay()
    {
        // Esperamos a que esté preparado
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        videoPlayer.Play();
    }

    public void StopBackground()
    {
        if (videoPlayer != null)
            videoPlayer.Stop();
    }
}
