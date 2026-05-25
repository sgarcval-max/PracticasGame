using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishCard : MonoBehaviour
{
    [Header("UI")]
    public Image fishIcon;
    public TextMeshProUGUI fishName;
    public TextMeshProUGUI fishDescription;
    public Button actionButton;
    public Button unequipButton;

    private int fishIndex;
    private BaseManager baseManager;

    // Tarjeta de mochila (pez sin domesticar)
    public void SetupBagCard(FishType type, int count, int index, BaseManager manager)
    {
        fishIndex = index;
        baseManager = manager;

        fishIcon.color = FishData.GetColor(type);

        fishName.text = count > 1 ?
            FishData.GetName(type) + " x" + count :
            FishData.GetName(type);

        fishDescription.text = "Sin domesticar\nPulsa para intentarlo";

        actionButton.gameObject.SetActive(true);
        unequipButton.gameObject.SetActive(false);

        // Botón naranja de domesticar
        actionButton.GetComponent<Image>().color = new Color(0.8f, 0.5f, 0.1f);
        actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Domesticar";

        actionButton.onClick.AddListener(Tame);
    }

    // Tarjeta de acuario (pez domesticado)
    public void SetupAquariumCard(FishType type, int count, int index, BaseManager manager, bool isEquipped)
    {
        fishIndex = index;
        baseManager = manager;

        fishIcon.color = FishData.GetColor(type);

        fishName.text = count > 1 ?
            FishData.GetName(type) + " x" + count :
            FishData.GetName(type);

        fishDescription.text = FishData.GetDescription(type);

        if (isEquipped)
        {
            actionButton.gameObject.SetActive(false);
            unequipButton.gameObject.SetActive(true);
            unequipButton.onClick.AddListener(Unequip);
        }
        else
        {
            actionButton.gameObject.SetActive(true);
            unequipButton.gameObject.SetActive(false);

            // Botón verde de equipar
            actionButton.GetComponent<Image>().color = new Color(0.2f, 0.7f, 0.2f);
            actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Equipar";

            actionButton.onClick.AddListener(Equip);
        }
    }

    void Tame() => baseManager.StartTaming(fishIndex);
    void Equip() => baseManager.EquipFish(fishIndex);
    void Unequip() => baseManager.UnequipFish(fishIndex);
}