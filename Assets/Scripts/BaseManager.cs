using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class BaseManager : MonoBehaviour
{
    [Header("Mission")]
    public TextMeshProUGUI missionText;

    [Header("Mochila - Peces capturados")]
    public GameObject bagPanel; // ASIGNAR EL PANEL NUEVO
    public Button openBagButton; // BOTÓN CON EL ICONO DE MOCHILA
    public Button closeBagButton; // BOTÓN "X" PARA CERRAR
    public Transform bagScrollContent;
    public GameObject fishCardPrefab;

    [Header("Acuario - Peces domesticados")]
    public Transform aquariumScrollContent;

    [Header("Equipped Panel")]
    public TextMeshProUGUI equippedTitle;
    public Image equippedSlot1;
    public Image equippedSlot2;
    public Image equippedSlot3;
    public TextMeshProUGUI equippedSlot1Text;
    public TextMeshProUGUI equippedSlot2Text;
    public TextMeshProUGUI equippedSlot3Text;

    [Header("Buttons")]
    public Button playButton;

    [Header("Opciones")]
    public GameObject optionsPanel;
    public Button optionsButton;
    public Button optionsBackButton;

    private TamingMinigame tamingMinigame;

    void Start()
    {
        playButton.onClick.AddListener(GoToSea);
        tamingMinigame = FindFirstObjectByType<TamingMinigame>();

        // Nuevos listeners para la mochila
        if (openBagButton != null) openBagButton.onClick.AddListener(OpenBag);
        if (closeBagButton != null) closeBagButton.onClick.AddListener(CloseBag);

        optionsButton.onClick.AddListener(OpenOptions);
        optionsBackButton.onClick.AddListener(CloseOptions);

        optionsPanel.SetActive(false);
        if (bagPanel != null) bagPanel.SetActive(false); // Empezar cerrada

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

    // MÉTODOS NUEVOS PARA EL PANEL
    public void OpenBag() { bagPanel.SetActive(true); RefreshUI(); }
    public void CloseBag() { bagPanel.SetActive(false); }

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

        // Agrupamos por tipo para las cartas
        Dictionary<FishType, int> fishCount = new Dictionary<FishType, int>();
        foreach (FishType fish in GameManager.Instance.caughtFish)
        {
            if (fishCount.ContainsKey(fish)) fishCount[fish]++;
            else fishCount[fish] = 1;
        }

        // Creamos las cartas basándonos en los tipos únicos capturados
        List<FishType> unique = GetUniqueCaughtFish();
        for (int i = 0; i < unique.Count; i++)
        {
            FishType type = unique[i];
            GameObject card = Instantiate(fishCardPrefab, bagScrollContent);
            FishCard fishCard = card.GetComponent<FishCard>();
            // Pasamos el índice de la lista UNIQUE para que StartTaming no falle
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

    void OpenOptions() => optionsPanel.SetActive(true);
    void CloseOptions() => optionsPanel.SetActive(false);
}