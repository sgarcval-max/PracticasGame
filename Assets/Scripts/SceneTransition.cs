using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Transición Base → Mar")]
    public VideoClip baseToGameOut;
    public VideoClip baseToGameIn;

    [Header("Transición Mar → Base")]
    public VideoClip gameToBaseOut;
    public VideoClip gameToBaseIn;

    private VideoPlayer videoPlayer;
    private RawImage videoImage;
    private GameObject transitionCanvas;
    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateTransitionCanvas();
    }

    void CreateTransitionCanvas()
    {
        transitionCanvas = new GameObject("TransitionCanvas");
        transitionCanvas.transform.SetParent(transform);

        Canvas canvas = transitionCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        transitionCanvas.AddComponent<CanvasScaler>();
        transitionCanvas.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("VideoImage");
        imageObj.transform.SetParent(transitionCanvas.transform, false);
        videoImage = imageObj.AddComponent<RawImage>();
        videoImage.color = Color.white;

        RectTransform rt = imageObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        RenderTexture renderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        videoImage.texture = renderTexture;

        videoPlayer = transitionCanvas.AddComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.isLooping = false;
        videoPlayer.playOnAwake = false;
        videoPlayer.skipOnDrop = true;

        // AudioSource para el sonido del video
        AudioSource videoAudio = transitionCanvas.AddComponent<AudioSource>();
        videoAudio.playOnAwake = false;
        videoPlayer.SetTargetAudioSource(0, videoAudio);

        transitionCanvas.SetActive(false);
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) { }

    public void TransitionToGame()
    {
        if (isTransitioning) return;
        StartCoroutine(DoTransition("GameScene", baseToGameOut, baseToGameIn));
    }

    public void TransitionToBase()
    {
        if (isTransitioning) return;
        StartCoroutine(DoTransition("BaseScene", gameToBaseOut, gameToBaseIn));
    }

    public void TransitionToMenu()
    {
        if (isTransitioning) return;
        StartCoroutine(DoTransition("MainMenu", gameToBaseOut, gameToBaseIn));
    }

    public void TransitionToMenuWithFade()
    {
        if (isTransitioning) return;
        StartCoroutine(FadeTransition("MainMenu"));
    }

    IEnumerator DoTransition(string sceneName, VideoClip outClip, VideoClip inClip)
    {
        isTransitioning = true;
        transitionCanvas.SetActive(true);

        // Fade out de música
        if (AudioManager.Instance != null)
            yield return StartCoroutine(AudioManager.Instance.FadeOutMusic(0.3f));

        // Video OUT
        if (outClip != null)
        {
            videoPlayer.clip = outClip;
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);
            videoPlayer.Play();

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
            asyncLoad.allowSceneActivation = true;
            yield return new WaitUntil(() => asyncLoad.isDone);
        }
        else
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            yield return new WaitForSecondsRealtime(0.3f);
            asyncLoad.allowSceneActivation = true;
            yield return new WaitUntil(() => asyncLoad.isDone);
        }

        yield return new WaitForEndOfFrame();

        // Video IN
        if (inClip != null)
        {
            videoPlayer.clip = inClip;
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);
            videoPlayer.Play();
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.3f);
        }

        transitionCanvas.SetActive(false);

        // Fade in de música en la nueva escena
        if (AudioManager.Instance != null)
            yield return StartCoroutine(AudioManager.Instance.FadeInMusic(0.5f));

        isTransitioning = false;
    }

    IEnumerator FadeTransition(string sceneName)
    {
        isTransitioning = true;

        // Creamos un canvas separado solo para el fade encima de todo
        GameObject fadeCanvas = new GameObject("FadeCanvas");
        Canvas fc = fadeCanvas.AddComponent<Canvas>();
        fc.renderMode = RenderMode.ScreenSpaceOverlay;
        fc.sortingOrder = 9999;
        fadeCanvas.AddComponent<CanvasScaler>();
        fadeCanvas.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(fadeCanvas);

        // Imagen negra que ocupa toda la pantalla
        GameObject blackObj = new GameObject("BlackScreen");
        blackObj.transform.SetParent(fadeCanvas.transform, false);
        UnityEngine.UI.Image blackImage = blackObj.AddComponent<UnityEngine.UI.Image>();
        blackImage.color = new Color(0, 0, 0, 0);
        blackImage.raycastTarget = true;

        RectTransform blackRt = blackObj.GetComponent<RectTransform>();
        blackRt.anchorMin = Vector2.zero;
        blackRt.anchorMax = Vector2.one;
        blackRt.offsetMin = Vector2.zero;
        blackRt.offsetMax = Vector2.zero;

        // Fade out música
        if (AudioManager.Instance != null)
            yield return StartCoroutine(AudioManager.Instance.FadeOutMusic(0.5f));

        // Fade a negro en GameScene
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.unscaledDeltaTime;
            blackImage.color = new Color(0, 0, 0, timer / 0.5f);
            yield return null;
        }
        blackImage.color = Color.black;

        // Cargamos MainMenu
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSecondsRealtime(0.3f);
        asyncLoad.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncLoad.isDone);

        yield return new WaitForEndOfFrame();

        // Fade desde negro en MainMenu
        timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.unscaledDeltaTime;
            blackImage.color = new Color(0, 0, 0, 1 - timer / 0.5f);
            yield return null;
        }

        Destroy(fadeCanvas);

        // Fade in música
        if (AudioManager.Instance != null)
            yield return StartCoroutine(AudioManager.Instance.FadeInMusic(0.5f));

        isTransitioning = false;
    }

    public bool IsTransitioning()
    {
        return isTransitioning;
    }
}