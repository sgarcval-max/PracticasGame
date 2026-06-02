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

    [Header("UI & Escenario")]
    public GameObject baseCanvas;
    public GameObject worldLevel;

    [Header("Audio & Config")]
    public float fadeDuration = 1f;

    void Awake()
    {
        // 1. Forzamos que el video esté listo para activarse
        if (videoPlayer != null) videoPlayer.gameObject.SetActive(true);

        // 2. Leemos la condición
        bool comingFromMenu = PlayerPrefs.GetInt("ComingFromMenu", 0) == 1;
        Debug.Log("¿Viene del menú?: " + comingFromMenu);

        if (comingFromMenu)
        {
            // MODO CINEMÁTICA
            PlayerPrefs.SetInt("ComingFromMenu", 0);
            PlayerPrefs.Save();

            // Apagamos todo lo que NO es cine
            baseCanvas.SetActive(false);
            if (worldLevel != null) worldLevel.SetActive(false);

            // Encendemos el Canvas de cine
            cinematicCanvas.SetActive(true);

            StartCoroutine(PlayCinematic());
        }
        else
        {
            // MODO JUEGO DIRECTO
            cinematicCanvas.SetActive(false);
            baseCanvas.SetActive(true);
            if (worldLevel != null) worldLevel.SetActive(true);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.isCinematicPlaying = false;
                AudioManager.Instance.ApplyVolumes();
                // Si venimos de reinicio, la música debe sonar
                if (!AudioManager.Instance.musicSource.isPlaying)
                    AudioManager.Instance.musicSource.Play();
            }
        }
    }

    IEnumerator PlayCinematic()
    {
        Debug.Log("Iniciando Corrutina de Video...");

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.isCinematicPlaying = true;
            AudioManager.Instance.musicSource.Stop(); // Silencio total al inicio
            AudioManager.Instance.musicSource.volume = 0f;
        }

        // Preparamos el audio del video
        if (AudioManager.Instance != null)
        {
            videoPlayer.SetDirectAudioVolume(0,
                AudioManager.Instance.masterVolume * AudioManager.Instance.cinematicVolume);
        }

        // Preparar y esperar al video
        videoPlayer.Prepare();

        // Esperamos máximo 5 segundos para que no se quede colgado si el video falla
        float timeout = 0f;
        while (!videoPlayer.isPrepared && timeout < 5f)
        {
            timeout += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Video preparado, dándole al Play");
        videoPlayer.Play();

        // Esperar un frame para que isPlaying se vuelva true
        yield return new WaitForEndOfFrame();

        // Esperar hasta que el video termine
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }

        Debug.Log("Video terminado.");
        EndCinematic();
    }

    void EndCinematic()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.isCinematicPlaying = false;

        cinematicCanvas.SetActive(false);
        baseCanvas.SetActive(true);
        if (worldLevel != null) worldLevel.SetActive(true);

        StartCoroutine(FadeInMusic());
    }

    IEnumerator FadeInMusic()
    {
        if (AudioManager.Instance == null) yield break;

        float targetVolume = AudioManager.Instance.masterVolume * AudioManager.Instance.musicVolume;

        AudioManager.Instance.musicSource.volume = 0f;
        AudioManager.Instance.musicSource.Play(); // Iniciamos la música AQUÍ

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            AudioManager.Instance.musicSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeDuration);
            yield return null;
        }

        AudioManager.Instance.musicSource.volume = targetVolume;
        AudioManager.Instance.ApplyVolumes();
    }
}