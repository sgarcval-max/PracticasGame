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

    [Header("Highlights por objeto")]
    public GameObject highlightMissionBoard;
    public GameObject highlightAquarium;
    public GameObject highlightBag;
    public GameObject highlightTaming;
    public GameObject highlightEquip;

    private HashSet<string> completedActions = new HashSet<string>();
    private bool isPanelOpen = false;
    private int totalActions = 6;
    private Coroutine highlightCoroutine;
    private GameObject currentHighlight;

    private const string TUTORIAL_KEY = "BaseTutorialSeen";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tutorialPanel.SetActive(false);
        HideAllHighlights();
        closeButton.onClick.AddListener(ClosePanel);

        // Cargamos las acciones completadas desde PlayerPrefs
        LoadCompletedActions();

        if (!HasSeenAction("entrance"))
            TriggerAction("entrance", "La Base",
                "Bienvenido a tu base!\nAquí podrás gestionar tus peces,\nver tus misiones y prepararte para volver al mar.");
    }

    void LoadCompletedActions()
    {
        string[] actions = { "entrance", "bag", "taming", "aquarium", "equip", "mission" };
        foreach (string action in actions)
        {
            if (HasSeenAction(action))
                completedActions.Add(action);
        }
    }

    void HideAllHighlights()
    {
        if (highlightMissionBoard != null) highlightMissionBoard.SetActive(false);
        if (highlightAquarium != null) highlightAquarium.SetActive(false);
        if (highlightBag != null) highlightBag.SetActive(false);
        if (highlightTaming != null) highlightTaming.SetActive(false);
        if (highlightEquip != null) highlightEquip.SetActive(false);
    }

    GameObject GetHighlightForAction(string actionId)
    {
        switch (actionId)
        {
            case "mission": return highlightMissionBoard;
            case "aquarium": return highlightAquarium;
            case "bag": return highlightBag;
            case "taming": return highlightTaming;
            case "equip": return highlightEquip;
            default: return null;
        }
    }

    public void TriggerAction(string actionId, string title, string description, RectTransform rectTarget)
    {
        if (isPanelOpen) return;
        if (HasSeenAction(actionId)) return;

        SaveAction(actionId);
        completedActions.Add(actionId);
        ShowPanel(title, description, actionId);
    }

    public void TriggerAction(string actionId, string title, string description, Transform worldTarget)
    {
        if (isPanelOpen) return;
        if (HasSeenAction(actionId)) return;

        SaveAction(actionId);
        completedActions.Add(actionId);
        ShowPanel(title, description, actionId);
    }

    public void TriggerAction(string actionId, string title, string description)
    {
        if (isPanelOpen) return;
        if (HasSeenAction(actionId)) return;

        SaveAction(actionId);
        completedActions.Add(actionId);
        ShowPanel(title, description, actionId);
    }

    void ShowPanel(string title, string description, string actionId)
    {
        isPanelOpen = true;
        tutorialTitle.text = title;
        tutorialText.text = description;

        if (tutorialCounter != null)
            tutorialCounter.text = completedActions.Count + "/" + totalActions;

        // Activamos el highlight correspondiente
        HideAllHighlights();
        currentHighlight = GetHighlightForAction(actionId);
        if (currentHighlight != null)
        {
            currentHighlight.SetActive(true);
            if (highlightCoroutine != null) StopCoroutine(highlightCoroutine);
            highlightCoroutine = StartCoroutine(PulseHighlight(currentHighlight));
        }

        tutorialPanel.SetActive(true);
    }

    IEnumerator PulseHighlight(GameObject highlight)
    {
        Image img = highlight.GetComponent<Image>();
        if (img == null) yield break;

        Color originalColor = img.color;

        while (isPanelOpen)
        {
            float t = Mathf.Sin(Time.unscaledTime * 3f) * 0.5f + 0.5f;
            img.color = new Color(originalColor.r, originalColor.g, originalColor.b,
                Mathf.Lerp(0.1f, 0.8f, t));
            yield return null;
        }

        img.color = originalColor;
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

        HideAllHighlights();

        // Después de la bienvenida → tablón de misiones
        if (completedActions.Contains("entrance") && !HasSeenAction("mission"))
        {
            TriggerAction("mission", "Tablón de Misiones",
                "Aquí puedes ver tu misión actual.\nNecesitas recoger 5 tesoros del mar.\nLos tesoros aparecen aleatoriamente en cada oleada.");
            return;
        }

        // Después del tablón → mochila automáticamente
        if (completedActions.Contains("mission") && !HasSeenAction("bag"))
        {
            BaseManager bm = FindFirstObjectByType<BaseManager>();
            TriggerAction("bag", "Mochila",
                "Aquí están los peces que has capturado en el mar.\nTienes que domesticarlos para poder usarlos.\nPulsa Domesticar para intentarlo!",
                bm?.bagButtonRect);
            return;
        }
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