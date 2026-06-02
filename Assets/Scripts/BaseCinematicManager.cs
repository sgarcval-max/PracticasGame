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

    [Header("Fade Blanco")]
    public CanvasGroup fadeCanvasGroup; // Arrastra aquí el Canvas Group de la FadeImage
    public float fadeOutDuration = 1.5f; // Duración del paso de blanco a transparente

    [Header("UI & Escenario")]
    public GameObject baseCanvas;
    public GameObject worldLevel;

    [Header("Audio & Config")]
    public float musicFadeDuration = 1f;

    void Awake()
    {
        if (videoPlayer != null) videoPlayer.gameObject.SetActive(true);

        // Al empezar, el fade blanco debe estar totalmente transparente
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0f;

        bool comingFromMenu = PlayerPrefs.GetInt("ComingFromMenu", 0) == 1;

        if (comingFromMenu)
        {
            PlayerPrefs.SetInt("ComingFromMenu", 0);
            PlayerPrefs.Save();

            baseCanvas.SetActive(false);
            if (worldLevel != null) worldLevel.SetActive(false);
            cinematicCanvas.SetActive(true);

            StartCoroutine(PlayCinematic());
        }
        else
        {
            cinematicCanvas.SetActive(false);
            baseCanvas.SetActive(true);
            if (worldLevel != null) worldLevel.SetActive(true);
        }
    }

    IEnumerator PlayCinematic()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.isCinematicPlaying = true;
            AudioManager.Instance.musicSource.Stop();
            AudioManager.Instance.musicSource.volume = 0f;
        }

        if (AudioManager.Instance != null)
        {
            videoPlayer.SetDirectAudioVolume(0,
                AudioManager.Instance.masterVolume * AudioManager.Instance.cinematicVolume);
        }

        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared) yield return null;

        videoPlayer.Play();

        yield return new WaitForEndOfFrame();
        while (videoPlayer.isPlaying) yield return null;

        // --- ¡EL VIDEO HA TERMINADO! ---

        // 1. Ponemos la pantalla en blanco al instante
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 1f;

        // 2. Esperamos un mini momento con la pantalla en blanco
        yield return new WaitForSeconds(0.2f);

        EndCinematic();
    }

    void EndCinematic()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.isCinematicPlaying = false;

        // Quitamos el video de fondo
        videoPlayer.gameObject.SetActive(false);

        // Activamos el juego
        baseCanvas.SetActive(true);
        if (worldLevel != null) worldLevel.SetActive(true);

        // 3. Iniciamos el efecto de desvanecer el blanco y subir la música
        StartCoroutine(FadeOutWhite());
        StartCoroutine(FadeInMusic());
    }

    IEnumerator FadeOutWhite()
    {
        if (fadeCanvasGroup == null) yield break;

        float timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            // Va de 1 (blanco sólido) a 0 (transparente)
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        // Apagamos el canvas de cine definitivamente
        cinematicCanvas.SetActive(false);
    }

    IEnumerator FadeInMusic()
    {
        if (AudioManager.Instance == null) yield break;

        float targetVolume = AudioManager.Instance.masterVolume * AudioManager.Instance.musicVolume;
        AudioManager.Instance.musicSource.volume = 0f;
        AudioManager.Instance.musicSource.Play();

        float timer = 0f;
        while (timer < musicFadeDuration)
        {
            timer += Time.deltaTime;
            AudioManager.Instance.musicSource.volume = Mathf.Lerp(0f, targetVolume, timer / musicFadeDuration);
            yield return null;
        }

        AudioManager.Instance.musicSource.volume = targetVolume;
        AudioManager.Instance.ApplyVolumes();
    }
}