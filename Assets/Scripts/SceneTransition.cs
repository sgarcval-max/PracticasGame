using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Videos")]
    public VideoClip transitionOut;
    public VideoClip transitionIn;

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

        // RenderTexture con canal alpha para transparencia
        RenderTexture renderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        renderTexture.antiAliasing = 1;
        videoImage.texture = renderTexture;

        videoPlayer = transitionCanvas.AddComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.isLooping = false;
        videoPlayer.playOnAwake = false;

        // Importante para transparencia en WebM
        videoPlayer.skipOnDrop = true;

        transitionCanvas.SetActive(false);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    public void TransitionToScene(string sceneName)
    {
        if (isTransitioning) return;
        StartCoroutine(DoTransition(sceneName));
    }

    IEnumerator DoTransition(string sceneName)
    {
        isTransitioning = true;
        transitionCanvas.SetActive(true);
        Debug.Log("Transicion iniciada");

        if (transitionOut != null)
        {
            Debug.Log("Reproduciendo video OUT");
            videoPlayer.clip = transitionOut;
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);
            Debug.Log("Video OUT preparado, reproduciendo...");
            videoPlayer.Play();
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
            Debug.Log("Video OUT terminado");
        }
        else
        {
            Debug.Log("No hay video OUT asignado");
            yield return new WaitForSeconds(0.5f);
        }

        SceneManager.LoadScene(sceneName);
        yield return null;

        if (transitionIn != null)
        {
            Debug.Log("Reproduciendo video IN");
            videoPlayer.clip = transitionIn;
            videoPlayer.Prepare();
            yield return new WaitUntil(() => videoPlayer.isPrepared);
            videoPlayer.Play();
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
            Debug.Log("Video IN terminado");
        }
        else
        {
            Debug.Log("No hay video IN asignado");
            yield return new WaitForSeconds(0.5f);
        }

        transitionCanvas.SetActive(false);
        isTransitioning = false;
    }
}