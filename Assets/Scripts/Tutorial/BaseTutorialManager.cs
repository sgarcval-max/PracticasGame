using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    private HashSet<string> completedActions = new HashSet<string>();
    private bool isPanelOpen = false;
    private int totalActions = 6;

    // Clave para guardar si ya se ha visto el tutorial
    private const string TUTORIAL_KEY = "BaseTutorialSeen";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        tutorialPanel.SetActive(false);
        closeButton.onClick.AddListener(ClosePanel);

        // Mostramos el tutorial de entrada siempre la primera vez
        if (!HasSeenAction("entrance"))
            TriggerAction("entrance", "La Base",
                "Bienvenido a tu base!\nAquí podrás gestionar tus peces,\nver tus misiones y prepararte para ir al mar.");
    }

    public void TriggerAction(string actionId, string title, string description)
    {
        if (isPanelOpen) return;
        if (HasSeenAction(actionId)) return;

        SaveAction(actionId);
        completedActions.Add(actionId);
        ShowPanel(title, description);
    }

    void ShowPanel(string title, string description)
    {
        isPanelOpen = true;
        tutorialTitle.text = title;
        tutorialText.text = description;

        if (tutorialCounter != null)
            tutorialCounter.text = completedActions.Count + "/" + totalActions;

        tutorialPanel.SetActive(true);
    }

    void ClosePanel()
    {
        isPanelOpen = false;
        tutorialPanel.SetActive(false);
    }

    public bool IsPanelOpen()
    {
        return isPanelOpen;
    }

    // Guardamos con PlayerPrefs para que solo salga una vez
    void SaveAction(string actionId)
    {
        PlayerPrefs.SetInt(TUTORIAL_KEY + actionId, 1);
        PlayerPrefs.Save();
    }

    bool HasSeenAction(string actionId)
    {
        return PlayerPrefs.GetInt(TUTORIAL_KEY + actionId, 0) == 1;
    }

    // Para resetear el tutorial si quieres verlo de nuevo
    public void ResetTutorial()
    {
        string[] actions = { "entrance", "bag", "taming", "aquarium", "equip", "mission" };
        foreach (string action in actions)
            PlayerPrefs.DeleteKey(TUTORIAL_KEY + action);
        PlayerPrefs.Save();
    }
}