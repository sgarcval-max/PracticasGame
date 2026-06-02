using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BaseManager : MonoBehaviour
{
    [Header("Mission")]
    public TextMeshProUGUI missionText;

    [Header("Mochila - Panel y Animación")]
    public GameObject bagPanel;
    public Button openBagButton;
    public Button closeBagButton;
    public float animationSpeed = 8f;
    private Coroutine bagCoroutine;

    [Header("Mochila - Contenido")]
    public Transform bagScrollContent;
    public GameObject fishCardPrefab;

    [Header("Equipped Panel")]
    public TextMeshProUGUI equippedTitle;
    public Image equippedSlot1;
    public Image equippedSlot2;
    public Image equippedSlot3;
    public TextMeshProUGUI equippedSlot1Text;
    public TextMeshProUGUI equippedSlot2Text;
    public TextMeshProUGUI equippedSlot3Text;

    [Header("Opciones")]
    public GameObject optionsPanel;
    public Button optionsButton;
    public Button optionsBackButton;

    [Header("Navegación")]
    public Button playButton;

    private TamingMinigame tamingMinigame;

    void Start()
    {
        tamingMinigame = FindFirstObjectByType<TamingMinigame>();

        playButton.onClick.AddListener(GoToSea);
        optionsButton.onClick.AddListener(() => optionsPanel.SetActive(true));
        optionsBackButton.onClick.AddListener(() => optionsPanel.SetActive(false));

        openBagButton.onClick.AddListener(OpenBag);
        closeBagButton.onClick.AddListener(CloseBag);

        optionsPanel.SetActive(false);

        bagPanel.SetActive(false);
        bagPanel.GetComponent<RectTransform>().localScale = Vector3.zero;

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (GameManager.Instance == null) return;

        UpdateMission();
        UpdateBag();
        UpdateAquarium();
        UpdateEquippedPanel();
    }

    // --- LÓGICA DE ANIMACIÓN Y AUDIO DE LA MOCHILA ---

    public void OpenBag()
    {
        if (bagCoroutine != null) StopCoroutine(bagCoroutine);

        // REPRODUCIR SONIDO AL ABRIR
        if (AudioManager.Instance != null && AudioManager.Instance.bagOpenSound != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.bagOpenSound);
        }

        bagPanel.SetActive(true);
        RefreshUI();
        bagCoroutine = StartCoroutine(AnimateBag(Vector3.one));
    }

    public void CloseBag()
    {
        if (bagCoroutine != null) StopCoroutine(bagCoroutine);

        // REPRODUCIR SONIDO AL CERRAR
        if (AudioManager.Instance != null && AudioManager.Instance.bagCloseSound != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.bagCloseSound);
        }

        bagCoroutine = StartCoroutine(AnimateBag(Vector3.zero, () => {
            bagPanel.SetActive(false);
        }));
    }

    private IEnumerator AnimateBag(Vector3 targetScale, System.Action onComplete = null)
    {
        RectTransform rect = bagPanel.GetComponent<RectTransform>();

        while (Vector3.Distance(rect.localScale, targetScale) > 0.005f)
        {
            rect.localScale = Vector3.Lerp(rect.localScale, targetScale, Time.deltaTime * animationSpeed);
            yield return null;
        }

        rect.localScale = targetScale;
        onComplete?.Invoke();
    }

    // --- ACTUALIZACIÓN DE UI ---

    void UpdateMission()
    {
        if (missionText == null) return;
        int collected = GameManager.Instance.collectedTreasure;
        int total = 5;

        if (GameManager.Instance.missionCompleted)
        {
            missionText.text = "Misión:\n" + GameManager.Instance.currentMission + "\n\n " + collected + "/" + total + "\n\n✓ COMPLETADA!";
            missionText.color = new Color(0.2f, 0.8f, 0.2f);
        }
        else
        {
            missionText.text = "Misión:\n" + GameManager.Instance.currentMission + "\n\n " + collected + "/" + total + "\n\nPendiente...";
            missionText.color = Color.white;
        }
    }

    void UpdateBag()
    {
        if (bagScrollContent == null) return;

        foreach (Transform child in bagScrollContent)
            Destroy(child.gameObject);

        Dictionary<FishType, int> fishCount = new Dictionary<FishType, int>();
        foreach (FishType fish in GameManager.Instance.caughtFish)
        {
            if (fishCount.ContainsKey(fish)) fishCount[fish]++;
            else fishCount[fish] = 1;
        }

        List<FishType> unique = GetUniqueCaughtFish();
        for (int i = 0; i < unique.Count; i++)
        {
            FishType type = unique[i];
            GameObject card = Instantiate(fishCardPrefab, bagScrollContent);
            FishCard fishCard = card.GetComponent<FishCard>();
            fishCard.SetupBagCard(type, fishCount[type], i, this);
        }
    }

    void UpdateAquarium()
    {
        AquariumManager aquariumManager = FindFirstObjectByType<AquariumManager>();
        aquariumManager?.RefreshAquarium();
    }

    void UpdateEquippedPanel()
    {
        List<FishType> equipped = GameManager.Instance.equippedFish;
        equippedTitle.text = "Peces Equipados (" + equipped.Count + "/3)";

        UpdateSlot(equippedSlot1, equippedSlot1Text, equipped, 0);
        UpdateSlot(equippedSlot2, equippedSlot2Text, equipped, 1);
        UpdateSlot(equippedSlot3, equippedSlot3Text, equipped, 2);
    }

    void UpdateSlot(Image slotImage, TextMeshProUGUI slotText, List<FishType> equipped, int index)
    {
        if (index < equipped.Count)
        {
            FishType fish = equipped[index];
            slotImage.color = FishData.GetColor(fish);
            slotText.text = FishData.GetName(fish) + "\n<size=14>" + FishData.GetDescription(fish) + "</size>";
        }
        else
        {
            slotImage.color = new Color(0.2f, 0.2f, 0.2f);
            slotText.text = "Vacío\nEquipa un pez";
        }
    }

    // --- ACCIONES ---

    public void StartTaming(int fishIndex)
    {
        List<FishType> uniqueFish = GetUniqueCaughtFish();
        if (fishIndex >= uniqueFish.Count) return;

        FishType fish = uniqueFish[fishIndex];

        tamingMinigame?.StartMinigame(fish, (success) =>
        {
            GameManager.Instance.caughtFish.Remove(fish);
            if (success) GameManager.Instance.tamedFish.Add(fish);
            RefreshUI();
        });
    }

    public void EquipFish(int fishIndex)
    {
        List<FishType> uniqueFish = GetUniqueTamedFish();
        if (fishIndex >= uniqueFish.Count) return;
        FishType fish = uniqueFish[fishIndex];

        if (GameManager.Instance.equippedFish.Count < 3 && !GameManager.Instance.equippedFish.Contains(fish))
        {
            GameManager.Instance.equippedFish.Add(fish);
            RefreshUI();
        }
    }

    public void UnequipFish(int fishIndex)
    {
        List<FishType> uniqueFish = GetUniqueTamedFish();
        if (fishIndex >= uniqueFish.Count) return;
        FishType fish = uniqueFish[fishIndex];
        GameManager.Instance.equippedFish.Remove(fish);
        RefreshUI();
    }

    // --- HELPERS ---

    List<FishType> GetUniqueCaughtFish()
    {
        List<FishType> unique = new List<FishType>();
        foreach (FishType fish in GameManager.Instance.caughtFish)
        {
            if (!unique.Contains(fish)) unique.Add(fish);
        }
        return unique;
    }

    List<FishType> GetUniqueTamedFish()
    {
        List<FishType> unique = new List<FishType>();
        foreach (FishType fish in GameManager.Instance.tamedFish)
        {
            if (!unique.Contains(fish)) unique.Add(fish);
        }
        return unique;
    }

    void GoToSea()
    {
        if (SceneTransition.Instance != null) SceneTransition.Instance.TransitionToGame();
        else SceneManager.LoadScene("GameScene");
    }
}