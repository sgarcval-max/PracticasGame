using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class BaseToMenuCinematic : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage videoImage;
    public GameObject cinematicCanvas;

    [Header("Menu")]
    public GameObject menuCanvas;

    void Start()
    {
        bool comingFromBase = PlayerPrefs.GetInt("ComingFromBase", 0) == 1;

        if (comingFromBase)
        {
            PlayerPrefs.SetInt("ComingFromBase", 0);
            PlayerPrefs.Save();

            menuCanvas.SetActive(false);
            cinematicCanvas.SetActive(true);

            StartCoroutine(PlayCinematic());
        }
        else
        {
            cinematicCanvas.SetActive(false);
        }
    }

    IEnumerator PlayCinematic()
    {
        // Silenciamos música
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.musicSource.volume = 0f;
            AudioManager.Instance.sfxSource.volume = 0f;
        }

        if (AudioManager.Instance != null)
            videoPlayer.SetDirectAudioVolume(0,
                AudioManager.Instance.masterVolume * AudioManager.Instance.cinematicVolume);

        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        videoPlayer.Play();

        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        EndCinematic();
    }

    void EndCinematic()
    {
        cinematicCanvas.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        menuCanvas.SetActive(true);

        // Fade in música
        StartCoroutine(FadeInMusic());
    }

    IEnumerator FadeInMusic()
    {
        if (AudioManager.Instance == null) yield break;

        float targetVolume = AudioManager.Instance.masterVolume * AudioManager.Instance.musicVolume;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            AudioManager.Instance.musicSource.volume = Mathf.Lerp(0f, targetVolume, timer);
            yield return null;
        }

        AudioManager.Instance.ApplyVolumes();
    }
}
