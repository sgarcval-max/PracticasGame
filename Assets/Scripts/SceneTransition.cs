using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Videos")]
    public VideoClip transitionOut;
    public VideoClip transitionIn;

    private VideoPlayer videoPlayer;
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

        // Creamos el canvas de transición por código
        // así no depende de la escena
        CreateTransitionCanvas();
    }

    void CreateTransitionCanvas()
    {
        // Creamos el canvas
        transitionCanvas = new GameObject("TransitionCanvas");
        transitionCanvas.transform.SetParent(transform);

        Canvas canvas = transitionCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        transitionCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        transitionCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Añadimos el VideoPlayer
        videoPlayer = transitionCanvas.AddComponent<VideoPlayer>();
        videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
        videoPlayer.targetCamera = Camera.main;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        videoPlayer.isLooping = false;

        // Lo ocultamos al inicio
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
        // Actualizamos la cámara del VideoPlayer al cargar nueva escena
        if (videoPlayer != null)
            videoPlayer.targetCamera = Camera.main;
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

        // Video de salida
        if (transitionOut != null)
        {
            videoPlayer.clip = transitionOut;
            videoPlayer.Play();
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Cargamos la escena
        SceneManager.LoadScene(sceneName);
        yield return null;

        // Video de entrada
        if (transitionIn != null)
        {
            videoPlayer.clip = transitionIn;
            videoPlayer.Play();
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        transitionCanvas.SetActive(false);
        isTransitioning = false;
    }
}
