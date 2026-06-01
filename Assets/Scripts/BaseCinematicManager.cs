using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class BaseCinematicManager : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage videoImage;
    public GameObject cinematicCanvas;

    [Header("Base")]
    public GameObject baseCanvas;

    [Header("Audio")]
    public float fadeDuration = 1f;

    void Start()
    {
        bool comingFromMenu = PlayerPrefs.GetInt("ComingFromMenu", 0) == 1;

        if (comingFromMenu)
        {
            PlayerPrefs.SetInt("ComingFromMenu", 0);
            PlayerPrefs.Save();

            baseCanvas.SetActive(false);
            cinematicCanvas.SetActive(true);

            StartCoroutine(PlayCinematic());
        }
        else
        {
            cinematicCanvas.SetActive(false);
            baseCanvas.SetActive(true);
        }
    }

    IEnumerator PlayCinematic()
    {
        // Silenciamos música y SFX
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.musicSource.volume = 0f;
            AudioManager.Instance.sfxSource.volume = 0f;
        }

        // Ajustamos volumen de la cinemática
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
        baseCanvas.SetActive(true);

        // Fade in de la música
        StartCoroutine(FadeInMusic());
    }

    IEnumerator FadeInMusic()
    {
        if (AudioManager.Instance == null) yield break;

        float targetVolume = AudioManager.Instance.masterVolume * AudioManager.Instance.musicVolume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            AudioManager.Instance.musicSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeDuration);
            yield return null;
        }

        AudioManager.Instance.musicSource.volume = targetVolume;
    }
}