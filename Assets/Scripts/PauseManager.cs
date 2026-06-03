using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public Button pauseButton;
    public Button resumeButton;
    public Button optionsButton;
    public Button optionsBackButton;
    public Button baseButton;
    public Button menuButton;

    private bool isPaused = false;

    void Start()
    {
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(Resume);
        optionsButton.onClick.AddListener(OpenOptions);
        optionsBackButton.onClick.AddListener(CloseOptions);
        baseButton.onClick.AddListener(GoToBase);
        menuButton.onClick.AddListener(GoToMenu);

        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void OpenOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
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
            SceneTransition.Instance.TransitionToMenuWithBlackFade();
        else
            SceneManager.LoadScene("MainMenu");
    }

    void SaveProgress()
    {
        DiverInventory inventory = FindFirstObjectByType<DiverInventory>();
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();

        if (GameManager.Instance != null)
            GameManager.Instance.SavePlayerData(inventory, waveManager);
    }
}