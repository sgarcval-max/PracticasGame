using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;
    public Button pauseButton;
    public Button resumeButton;
    public Button optionsButton;
    public Button baseButton;
    public Button menuButton;

    private bool isPaused = false;

    void Start()
    {
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(Resume);
        optionsButton.onClick.AddListener(OpenOptions);
        baseButton.onClick.AddListener(GoToBase);
        menuButton.onClick.AddListener(GoToMenu);

        pausePanel.SetActive(false);
    }

    void Update()
    {
        // También pausar con Escape
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);

        // Pausar o reanudar el tiempo del juego
        Time.timeScale = isPaused ? 0f : 1f;
    }

    void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void OpenOptions()
    {
        // De momento solo reanudamos
        // Luego conectamos con el panel de opciones
        Debug.Log("Opciones");
    }

    void GoToBase()
    {
        Time.timeScale = 1f;
        SaveProgress();
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionToBase();
        else
            SceneManager.LoadScene("BaseScene");
    }

    void GoToMenu()
    {
        Time.timeScale = 1f;
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionToMenu();
        else
            SceneManager.LoadScene("MainMenu");
    }

    void SaveProgress()
    {
        // Guardamos inventario y oleada
        DiverInventory inventory = FindFirstObjectByType<DiverInventory>();
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SavePlayerData(inventory, waveManager);
        }
    }
}