using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BaseTutorialManager : MonoBehaviour
{
    public static BaseTutorialManager Instance;

    [Header("Panel")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialTitle;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI tutorialCounter;
    public Button closeButton;

    [Header("Highlight")]
    public Image highlightImage;

    private HashSet<string> completedActions = new HashSet<string>();
    private bool isPanelOpen = false;
    private int totalActions = 6;
    private Coroutine highlightCoroutine;

    private const string TUTORIAL_KEY = "BaseTutorialSeen";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tutorialPanel.SetActive(false);

        if (highlightImage != null)
            highlightImage.gameObject.SetActive(false);

        closeButton.onClick.AddListener(ClosePanel);

        if (!HasSeenAction("entrance"))
            TriggerAction("entrance", "La Base",
                "Bienvenido a tu base!\nAquí podrás gestionar tus peces,\nver tus misiones y prepararte para volver al mar.",
                null);
    }

    public void TriggerAction(string actionId, string title, string description, RectTransform highlightTarget)
    {
        if (isPanelOpen) return;
        if (HasSeenAction(actionId)) return;

        SaveAction(actionId);
        completedActions.Add(actionId);
        ShowPanel(title, description, highlightTarget);
    }

    void ShowPanel(string title, string description, RectTransform highlightTarget)
    {
        isPanelOpen = true;
        tutorialTitle.text = title;
        tutorialText.text = description;

        if (tutorialCounter != null)
            tutorialCounter.text = completedActions.Count + "/" + totalActions;

        // Resaltamos el elemento
        if (highlightTarget != null && highlightImage != null)
        {
            highlightImage.gameObject.SetActive(true);
            highlightImage.rectTransform.position = highlightTarget.position;
            highlightImage.rectTransform.sizeDelta = highlightTarget.sizeDelta + new Vector2(20f, 20f);

            if (highlightCoroutine != null) StopCoroutine(highlightCoroutine);
            highlightCoroutine = StartCoroutine(PulseHighlight());
        }

        tutorialPanel.SetActive(true);
    }

    IEnumerator PulseHighlight()
    {
        if (highlightImage == null) yield break;

        while (isPanelOpen)
        {
            float t = Mathf.Sin(Time.unscaledTime * 3f) * 0.5f + 0.5f;
            highlightImage.color = new Color(1f, 1f, 0f, Mathf.Lerp(0.1f, 0.6f, t));
            yield return null;
        }

        highlightImage.color = new Color(1f, 1f, 0f, 0f);
        highlightImage.gameObject.SetActive(false);
    }

    void ClosePanel()
    {
        isPanelOpen = false;
        tutorialPanel.SetActive(false);

        if (highlightCoroutine != null)
        {
            StopCoroutine(highlightCoroutine);
            highlightCoroutine = null;
        }

        if (highlightImage != null)
            highlightImage.gameObject.SetActive(false);
    }

    public bool IsPanelOpen() => isPanelOpen;

    void SaveAction(string actionId)
    {
        PlayerPrefs.SetInt(TUTORIAL_KEY + actionId, 1);
        PlayerPrefs.Save();
    }

    bool HasSeenAction(string actionId)
    {
        return PlayerPrefs.GetInt(TUTORIAL_KEY + actionId, 0) == 1;
    }

    public void ResetTutorial()
    {
        string[] actions = { "entrance", "bag", "taming", "aquarium", "equip", "mission" };
        foreach (string action in actions)
            PlayerPrefs.DeleteKey(TUTORIAL_KEY + action);
        PlayerPrefs.Save();
    }
}