using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Panel de tutorial")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialTitle;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI tutorialCounter;
    public Button closeButton;

    [Header("Botón de fin")]
    public GameObject finishButton;

    private HashSet<string> completedActions = new HashSet<string>();
    private bool isPanelOpen = false;
    private int totalActions = 7;

    void Awake()
    {
        Instance = this;
        Debug.Log("TutorialManager Instance asignado");
    }

    void Start()
    {
        tutorialPanel.SetActive(false);
        finishButton.SetActive(false);
        closeButton.onClick.AddListener(ClosePanel);
        finishButton.GetComponent<Button>().onClick.AddListener(GoToMenu);

        // Guardamos los datos reales del jugador antes de empezar el tutorial
        SaveRealData();

        // Reseteamos los datos para que el tutorial no guarde nada
        ResetTutorialData();
    }

    // Guardamos los datos reales antes del tutorial
    void SaveRealData()
    {
        if (GameManager.Instance == null) return;

        PlayerPrefs.SetInt("RealTreasure", GameManager.Instance.collectedTreasure);
        PlayerPrefs.SetInt("RealWave", GameManager.Instance.currentWave);
        PlayerPrefs.SetString("RealMission", GameManager.Instance.currentMission);
        PlayerPrefs.SetInt("RealMissionCompleted", GameManager.Instance.missionCompleted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Reseteamos los datos para el tutorial
    void ResetTutorialData()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.caughtFish.Clear();
        GameManager.Instance.tamedFish.Clear();
        GameManager.Instance.equippedFish.Clear();
        GameManager.Instance.collectedTreasure = 0;
        GameManager.Instance.currentWave = 0;
        GameManager.Instance.missionCompleted = false;
        GameManager.Instance.caughtFishBackup.Clear();
        GameManager.Instance.collectedTreasureBackup = 0;
    }

    // Restauramos los datos reales al salir del tutorial
    void RestoreRealData()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.collectedTreasure = PlayerPrefs.GetInt("RealTreasure", 0);
        GameManager.Instance.currentWave = PlayerPrefs.GetInt("RealWave", 0);
        GameManager.Instance.currentMission = PlayerPrefs.GetString("RealMission", "Encuentra el cofre dorado");
        GameManager.Instance.missionCompleted = PlayerPrefs.GetInt("RealMissionCompleted", 0) == 1;
        GameManager.Instance.caughtFish.Clear();
        GameManager.Instance.tamedFish.Clear();
        GameManager.Instance.equippedFish.Clear();
    }

    public void TriggerAction(string actionId, string title, string description)
    {
        if (isPanelOpen) return;
        if (completedActions.Contains(actionId)) return;

        completedActions.Add(actionId);
        ShowPanel(title, description);
    }

    void ShowPanel(string title, string description)
    {
        isPanelOpen = true;
        tutorialTitle.text = title;
        tutorialText.text = description;

        // Actualizamos el contador
        if (tutorialCounter != null)
            tutorialCounter.text = completedActions.Count + "/" + totalActions;

        tutorialPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void ClosePanel()
    {
        isPanelOpen = false;
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;

        if (completedActions.Count >= totalActions)
            finishButton.SetActive(true);
    }

    public bool IsPanelOpen()
    {
        return isPanelOpen;
    }

    void GoToMenu()
    {
        Time.timeScale = 1f;

        // Restauramos los datos reales antes de salir
        RestoreRealData();

        StartCoroutine(FadeToMenu());
    }

    IEnumerator FadeToMenu()
    {
        GameObject fadeCanvas = new GameObject("FadeCanvas");
        Canvas fc = fadeCanvas.AddComponent<Canvas>();
        fc.renderMode = RenderMode.ScreenSpaceOverlay;
        fc.sortingOrder = 9999;
        fadeCanvas.AddComponent<CanvasScaler>();

        GameObject blackObj = new GameObject("BlackScreen");
        blackObj.transform.SetParent(fadeCanvas.transform, false);
        Image blackImage = blackObj.AddComponent<Image>();
        blackImage.color = new Color(0, 0, 0, 0);

        RectTransform rt = blackObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, timer / 0.5f);
            yield return null;
        }

        SceneManager.LoadScene("MainMenu");
    }
}