using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class BaseManager : MonoBehaviour
{
    [Header("Mission")]
    public TextMeshProUGUI missionText;

    [Header("Mochila - Peces capturados sin domesticar")]
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

    private TamingMinigame tamingMinigame;

    void Start()
    {
        playButton.onClick.AddListener(GoToSea);
        tamingMinigame = FindFirstObjectByType<TamingMinigame>();
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

    void UpdateMission()
    {
        if (missionText == null) return;

        int collected = GameManager.Instance.collectedTreasure;
        int total = 5;

        if (GameManager.Instance.missionCompleted)
        {
            missionText.text = "Misión:\n" + GameManager.Instance.currentMission +
                               "\n\n💎 " + collected + "/" + total +
                               "\n\n✓ COMPLETADA!";
            missionText.color = new Color(0.2f, 0.8f, 0.2f);
        }
        else
        {
            missionText.text = "Misión:\n" + GameManager.Instance.currentMission +
                               "\n\n💎 " + collected + "/" + total +
                               "\n\nPendiente...";
            missionText.color = Color.white;
        }
    }

    // Mochila: peces capturados sin domesticar
    void UpdateBag()
    {
        foreach (Transform child in bagScrollContent)
            Destroy(child.gameObject);

        Dictionary<FishType, int> fishCount = new Dictionary<FishType, int>();
        foreach (FishType fish in GameManager.Instance.caughtFish)
        {
            if (fishCount.ContainsKey(fish))
                fishCount[fish]++;
            else
                fishCount[fish] = 1;
        }

        int index = 0;
        foreach (KeyValuePair<FishType, int> entry in fishCount)
        {
            GameObject card = Instantiate(fishCardPrefab, bagScrollContent);
            FishCard fishCard = card.GetComponent<FishCard>();
            fishCard.SetupBagCard(entry.Key, entry.Value, index, this);
            index++;
        }
    }

    // Acuario: peces domesticados
    void UpdateAquarium()
    {
        AquariumManager aquariumManager = FindFirstObjectByType<AquariumManager>();
        aquariumManager?.RefreshAquarium();
    }

    void UpdateEquippedPanel()
    {
        List<FishType> equipped = GameManager.Instance.equippedFish;
        equippedTitle.text = "⚔️ Peces Equipados (" + equipped.Count + "/3)";

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

    // Iniciar minijuego de domesticación
    public void StartTaming(int fishIndex)
    {
        List<FishType> uniqueFish = GetUniqueCaughtFish();
        if (fishIndex >= uniqueFish.Count) return;

        FishType fish = uniqueFish[fishIndex];

        tamingMinigame?.StartMinigame(fish, (success) =>
        {
            if (success)
            {
                // Domesticado con éxito
                GameManager.Instance.caughtFish.Remove(fish);
                GameManager.Instance.tamedFish.Add(fish);
            }
            else
            {
                // Fallo, perdemos el pez
                GameManager.Instance.caughtFish.Remove(fish);
            }

            RefreshUI();
        });
    }

    public void EquipFish(int fishIndex)
    {
        List<FishType> uniqueFish = GetUniqueTamedFish();
        if (fishIndex >= uniqueFish.Count) return;

        FishType fish = uniqueFish[fishIndex];

        if (GameManager.Instance.equippedFish.Count >= 3)
        {
            Debug.Log("Ya tienes 3 peces equipados!");
            return;
        }

        if (!GameManager.Instance.equippedFish.Contains(fish))
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
            if (!unique.Contains(fish))
                unique.Add(fish);
        }
        return unique;
    }

    List<FishType> GetUniqueTamedFish()
    {
        List<FishType> unique = new List<FishType>();
        foreach (FishType fish in GameManager.Instance.tamedFish)
        {
            if (!unique.Contains(fish))
                unique.Add(fish);
        }
        return unique;
    }

    void GoToSea()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionToScene("GameScene");
        else
            SceneManager.LoadScene("GameScene");
    }
}