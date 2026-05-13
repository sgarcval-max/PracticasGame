using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class BaseManager : MonoBehaviour
{
    [Header("Mission")]
    public TextMeshProUGUI missionText;

    [Header("Fish Collection")]
    public Transform fishScrollContent;
    public GameObject fishCardPrefab;

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

    void Start()
    {
        playButton.onClick.AddListener(GoToSea);
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (GameManager.Instance == null) return;

        UpdateMission();
        UpdateFishCollection();
        UpdateEquippedPanel();
    }

    void UpdateMission()
    {
        if (missionText == null) return;

        if (GameManager.Instance.missionCompleted)
        {
            missionText.text = "Misión:\n" + GameManager.Instance.currentMission + "\n\n✓ COMPLETADA!";
            missionText.color = new Color(0.2f, 0.8f, 0.2f);
        }
        else
        {
            missionText.text = "Misión:\n" + GameManager.Instance.currentMission + "\n\nPendiente...";
            missionText.color = Color.white;
        }
    }

    void UpdateFishCollection()
    {
        // Limpiar tarjetas anteriores
        foreach (Transform child in fishScrollContent)
        {
            Destroy(child.gameObject);
        }

        // Crear una tarjeta por cada pez capturado
        List<FishType> allFish = GameManager.Instance.allFish;

        for (int i = 0; i < allFish.Count; i++)
        {
            GameObject card = Instantiate(fishCardPrefab, fishScrollContent);
            FishCard fishCard = card.GetComponent<FishCard>();

            bool isEquipped = GameManager.Instance.equippedFish.Contains(allFish[i]);
            fishCard.Setup(allFish[i], i, this, isEquipped);
        }

        if (allFish.Count == 0)
        {
            // Mensaje si no hay peces
            GameObject card = Instantiate(fishCardPrefab, fishScrollContent);
            FishCard fishCard = card.GetComponent<FishCard>();
        }
    }

    void UpdateEquippedPanel()
    {
        List<FishType> equipped = GameManager.Instance.equippedFish;

        // Actualizar título
        equippedTitle.text = "⚔️ Peces Equipados (" + equipped.Count + "/3)";

        // Actualizar slots
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

    public void EquipFish(int fishIndex)
    {
        if (GameManager.Instance.equippedFish.Count >= 3)
        {
            Debug.Log("Ya tienes 3 peces equipados!");
            return;
        }

        FishType fish = GameManager.Instance.allFish[fishIndex];

        if (!GameManager.Instance.equippedFish.Contains(fish))
        {
            GameManager.Instance.equippedFish.Add(fish);
            RefreshUI();
        }
    }

    public void UnequipFish(int fishIndex)
    {
        FishType fish = GameManager.Instance.allFish[fishIndex];
        GameManager.Instance.equippedFish.Remove(fish);
        RefreshUI();
    }

    void GoToSea()
    {
        SceneManager.LoadScene("GameScene");
    }
}