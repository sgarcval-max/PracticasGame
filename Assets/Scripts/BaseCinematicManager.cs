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

    [Header("Fade Blanco / Fondo Negro")]
    public CanvasGroup fadeCanvasGroup; // El Canvas Group de la imagen blanca de fundido
    public float fadeOutDuration = 1.5f;

    [Header("UI & Escenario")]
    public GameObject baseCanvas;  // La UI normal del juego (vida, oxigeno...)
    public GameObject worldLevel;  // El objeto que contiene todo el escenario/mapa

    [Header("Audio & Config")]
    public float musicFadeDuration = 1f;

    void Awake()
    {
        // --- PREVENCIÓN DE DESTELLOS (FLICKERING) ---
        // Apagamos TODO el escenario y la UI antes de que se renderice el primer frame
        if (worldLevel != null) worldLevel.SetActive(false);
        if (baseCanvas != null) baseCanvas.SetActive(false);

        // El canvas de la cinemática debe ser lo único visible
        if (cinematicCanvas != null) cinematicCanvas.SetActive(true);

        // El fade blanco empieza invisible para dejar ver el video
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0f;

        // Comprobamos si venimos del menú para saber si toca vídeo
        bool comingFromMenu = PlayerPrefs.GetInt("ComingFromMenu", 0) == 1;

        if (comingFromMenu)
        {
            // Limpiamos la bandera para que no se repita al morir/reiniciar
            PlayerPrefs.SetInt("ComingFromMenu", 0);
            PlayerPrefs.Save();

            StartCoroutine(PlayCinematic());
        }
        else
        {
            // Si entramos directamente (testeo), saltamos al juego
            EndCinematicDirectly();
        }
    }

    IEnumerator PlayCinematic()
    {
        // Parar música de fondo para que no se mezcle con el video
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.isCinematicPlaying = true;
            AudioManager.Instance.musicSource.Stop();
        }

        // Ponemos la imagen en negro mientras el video carga
        videoImage.color = Color.black;

        // Preparar el video
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared) yield return null;

        // El video está listo: restauramos color y damos Play
        videoImage.color = Color.white;
        videoPlayer.Play();

        // Pequeña espera para asegurar que el primer frame del video ya se está dibujando
        yield return new WaitForEndOfFrame();

        // Bucle que mantiene la cinemática activa mientras el video se reproduzca
        while (videoPlayer.isPlaying) yield return null;

        // --- FINAL DEL VIDEO ---
        // 1. Fundido a blanco instantáneo para ocultar el corte del video
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.2f);

        TerminarYActivarJuego();
    }

    void TerminarYActivarJuego()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.isCinematicPlaying = false;

        // Desactivamos el objeto del video
        videoPlayer.gameObject.SetActive(false);

        // ACTIVAMOS el escenario y la UI del juego
        if (baseCanvas != null) baseCanvas.SetActive(true);
        if (worldLevel != null) worldLevel.SetActive(true);

        // Iniciamos los efectos visuales y sonoros de entrada
        StartCoroutine(FadeOutWhite());
        StartCoroutine(FadeInMusic());
    }

    void EndCinematicDirectly()
    {
        if (cinematicCanvas != null) cinematicCanvas.SetActive(false);
        if (baseCanvas != null) baseCanvas.SetActive(true);
        if (worldLevel != null) worldLevel.SetActive(true);
    }

    IEnumerator FadeOutWhite()
    {
        if (fadeCanvasGroup == null) yield break;

        float timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            // Pasa de 1 (blanco) a 0 (transparente)
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        // Apagamos el canvas de cine definitivamente al terminar el fade
        if (cinematicCanvas != null) cinematicCanvas.SetActive(false);
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
    }
}