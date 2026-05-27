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

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    [Header("Configuración")]
    public float mouseIdleTime = 1.5f;

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
        if (hasPlayedCinematic)
        {
            SkipCinematic();
            return;
        }

        menuCanvas.SetActive(false);
        skipTextObject.SetActive(false);
        skipTextCanvasGroup.alpha = 0f;
        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.gameObject.SetActive(false);

        lastMousePos = Mouse.current.position.ReadValue();
        StartCoroutine(PlayCinematic());
    }

    void Update()
    {
        if (!isPlaying || isSkipping) return;

        Vector2 currentMousePos = Mouse.current.position.ReadValue();
        float mouseDelta = Vector2.Distance(currentMousePos, lastMousePos);
        lastMousePos = currentMousePos;

        if (mouseDelta > 2f)
        {
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
            mouseTimer -= Time.deltaTime;
            if (mouseTimer <= 0f)
            {
                isSkipTextVisible = false;
                StopCoroutine("FadeSkipText");
                StartCoroutine(FadeSkipText(false));
            }
        }

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

        // Si no se ha saltado mostramos el menú directamente sin fade
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
        if (!fadeIn) skipTextObject.SetActive(false);
    }

    IEnumerator SkipWithFade()
    {
        isSkipping = true;

        // Ocultamos el texto de saltar
        if (isSkipTextVisible)
        {
            isSkipTextVisible = false;
            skipTextObject.SetActive(false);
        }

        // El video sigue reproduciéndose mientras hacemos el fade a negro
        yield return StartCoroutine(FadeToBlack());

        // Solo paramos el video cuando ya está todo negro
        videoPlayer.Stop();

        EndCinematic();
    }

    IEnumerator FadeToMenu()
    {
        // Fundido a negro al terminar el video
        yield return StartCoroutine(FadeToBlack());
        EndCinematic();
    }

    IEnumerator FadeToBlack()
    {
        fadeImage.gameObject.SetActive(true);
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, timer / fadeDuration);
            yield return null;
        }

        fadeImage.color = Color.black;
    }

    void EndCinematic()
    {
        hasPlayedCinematic = true;
        isPlaying = false;

        videoImage.gameObject.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        skipTextObject.SetActive(false);

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