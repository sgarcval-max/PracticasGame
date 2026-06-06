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
                null, null);
    }

    public void TriggerAction(string actionId, string title, string description, RectTransform rectTarget)
    {
        TriggerAction(actionId, title, description, rectTarget, null);
    }

    public void TriggerAction(string actionId, string title, string description, Transform worldTarget)
    {
        TriggerAction(actionId, title, description, null, worldTarget);
    }

    public void TriggerAction(string actionId, string title, string description)
    {
        TriggerAction(actionId, title, description, null, null);
    }

    private void TriggerAction(string actionId, string title, string description, RectTransform rectTarget, Transform worldTarget)
    {
        if (isPanelOpen) return;
        if (HasSeenAction(actionId)) return;

        SaveAction(actionId);
        completedActions.Add(actionId);
        ShowPanel(title, description, rectTarget, worldTarget);
    }

    void ShowPanel(string title, string description, RectTransform rectTarget, Transform worldTarget)
    {
        isPanelOpen = true;
        tutorialTitle.text = title;
        tutorialText.text = description;

        if (tutorialCounter != null)
            tutorialCounter.text = completedActions.Count + "/" + totalActions;

        if (highlightImage != null)
        {
            if (rectTarget != null)
            {
                highlightImage.gameObject.SetActive(true);
                highlightImage.rectTransform.position = rectTarget.position;
                highlightImage.rectTransform.sizeDelta = rectTarget.rect.size + new Vector2(10f, 10f);
                highlightImage.rectTransform.pivot = rectTarget.pivot;

                if (highlightCoroutine != null) StopCoroutine(highlightCoroutine);
                highlightCoroutine = StartCoroutine(PulseHighlight());
            }
            else if (worldTarget != null)
            {
                highlightImage.gameObject.SetActive(true);

                if (highlightCoroutine != null) StopCoroutine(highlightCoroutine);
                highlightCoroutine = StartCoroutine(FollowWorldObject(worldTarget));
            }
            else
            {
                highlightImage.gameObject.SetActive(false);
            }
        }

        tutorialPanel.SetActive(true);
    }

    IEnumerator FollowWorldObject(Transform worldTarget)
    {
        if (highlightImage == null) yield break;

        Camera cam = Camera.main;

        while (isPanelOpen && worldTarget != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(worldTarget.position);
            highlightImage.rectTransform.position = screenPos;

            Vector3 size = worldTarget.localScale * 100f;
            highlightImage.rectTransform.sizeDelta = new Vector2(size.x + 10f, size.y + 10f);

            float t = Mathf.Sin(Time.unscaledTime * 3f) * 0.5f + 0.5f;
            highlightImage.color = new Color(1f, 1f, 0f, Mathf.Lerp(0.1f, 0.6f, t));

            yield return null;
        }

        highlightImage.color = new Color(1f, 1f, 0f, 0f);
        highlightImage.gameObject.SetActive(false);
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

        // Si acabamos de ver la bienvenida mostramos el tablón automáticamente
        if (completedActions.Contains("entrance") && !HasSeenAction("mission"))
        {
            BaseManager bm = FindFirstObjectByType<BaseManager>();
            if (bm != null)
            {
                TriggerAction("mission", "Tablón de Misiones",
                    "Aquí puedes ver tu misión actual.\nNecesitas recoger 5 tesoros del mar.\nLos tesoros aparecen aleatoriamente en cada oleada.",
                    bm.missionBoardTransform);
            }
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