using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class CinematicManager : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage videoImage;

    [Header("UI")]
    public GameObject skipTextObject;
    public TextMeshProUGUI skipText;
    public CanvasGroup skipTextCanvasGroup;

    [Header("Configuración")]
    public float mouseIdleTime = 1.5f;
    public float fadeDuration = 0.5f;

    // Para saber si ya se ha visto la cinemática
    private static bool hasPlayedCinematic = false;

    private float mouseTimer = 0f;
    private Vector2 lastMousePos;
    private bool isSkipTextVisible = false;
    private bool isPlaying = false;
    private bool isSkipping = false;

    [Header("Menu")]
    public GameObject menuCanvas;

    void Start()
    {
        // Si ya se ha visto la cinemática saltamos directamente al menú
        if (hasPlayedCinematic)
        {
            SkipCinematic();
            return;
        }

        menuCanvas.SetActive(false);
        skipTextObject.SetActive(false);
        skipTextCanvasGroup.alpha = 0f;

        lastMousePos = Mouse.current.position.ReadValue();

        StartCoroutine(PlayCinematic());
    }

    void Update()
    {
        if (!isPlaying || isSkipping) return;

        // Detectar movimiento del ratón
        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        float mouseDelta = Vector2.Distance(currentMousePos, lastMousePos);
        lastMousePos = currentMousePos;

        if (mouseDelta > 2f)
        {
            // El ratón se ha movido, mostramos el texto
            mouseTimer = mouseIdleTime;
            if (!isSkipTextVisible)
            {
                isSkipTextVisible = true;
                StopCoroutine("FadeSkipText");
                StartCoroutine(FadeSkipText(true));
            }
        }
        else if (isSkipTextVisible)
        {
            // El ratón está quieto, contamos el tiempo
            mouseTimer -= Time.deltaTime;
            if (mouseTimer <= 0f)
            {
                isSkipTextVisible = false;
                StopCoroutine("FadeSkipText");
                StartCoroutine(FadeSkipText(false));
            }
        }

        // Pulsar espacio para saltar
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartCoroutine(SkipWithFade());
        }
    }

    IEnumerator PlayCinematic()
    {
        isPlaying = true;

        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        videoPlayer.Play();

        // Esperamos a que termine el video
        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        // Si no se ha saltado mostramos el menú
        if (!isSkipping)
            EndCinematic();
    }

    IEnumerator FadeSkipText(bool fadeIn)
    {
        skipTextObject.SetActive(true);
        float start = fadeIn ? 0f : 1f;
        float end = fadeIn ? 1f : 0f;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            skipTextCanvasGroup.alpha = Mathf.Lerp(start, end, timer / fadeDuration);
            yield return null;
        }

        skipTextCanvasGroup.alpha = end;

        if (!fadeIn)
            skipTextObject.SetActive(false);
    }

    IEnumerator SkipWithFade()
    {
        isSkipping = true;
        videoPlayer.Stop();

        // Fade out del texto si está visible
        if (isSkipTextVisible)
            yield return StartCoroutine(FadeSkipText(false));

        EndCinematic();
    }

    void EndCinematic()
    {
        hasPlayedCinematic = true;
        isPlaying = false;

        // Desactivamos el video
        videoImage.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        skipTextObject.SetActive(false);

        // Activamos el menú principal
        menuCanvas.SetActive(true);
    }

    void SkipCinematic()
    {
        videoImage.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        skipTextObject.SetActive(false);
        menuCanvas.SetActive(true);
    }
}