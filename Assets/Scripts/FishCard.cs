using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishCard : MonoBehaviour
{
    [Header("UI")]
    public Image fishIcon;
    public TextMeshProUGUI fishName;
    public TextMeshProUGUI fishDescription;
    public Button equipButton;
    public Button unequipButton;

    private FishType fishType;
    private int fishIndex;
    private BaseManager baseManager;

    public void Setup(FishType type, int index, BaseManager manager, bool isEquipped, int count)
    {
        fishType = type;
        fishIndex = index;
        baseManager = manager;

        // Color del pez
        fishIcon.color = FishData.GetColor(type);

        // Nombre con contador si hay más de uno
        if (count > 1)
            fishName.text = FishData.GetName(type) + " x" + count;
        else
            fishName.text = FishData.GetName(type);

        // Descripción
        fishDescription.text = FishData.GetDescription(type);

        // Mostrar botón correcto
        if (isEquipped)
        {
            equipButton.gameObject.SetActive(false);
            unequipButton.gameObject.SetActive(true);
        }
        else
        {
            equipButton.gameObject.SetActive(true);
            unequipButton.gameObject.SetActive(false);
        }

        equipButton.onClick.AddListener(Equip);
        unequipButton.onClick.AddListener(Unequip);
    }

    void Equip()
    {
        baseManager.EquipFish(fishIndex);
    }

    void Unequip()
    {
        baseManager.UnequipFish(fishIndex);
    }
}