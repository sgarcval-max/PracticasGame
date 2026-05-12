using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityManager : MonoBehaviour
{
    private FishAbility[] slots = new FishAbility[3];

    private DiverInventory inventory;

    void Awake()
    {
        inventory = GetComponent<DiverInventory>();
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) TryUseSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) TryUseSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) TryUseSlot(2);
    }

    void TryUseSlot(int index)
    {
        if (slots[index] == null)
        {
            Debug.Log("Slot " + (index + 1) + " vacio");
            return;
        }

        slots[index].TryActivate();
    }

    public void AddAbility(FishType fishType)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = CreateAbility(fishType);
                Debug.Log("Habilidad equipada en slot " + (i + 1) + ": " + fishType);
                return;
            }
        }

        Debug.Log("No hay slots libres");
    }

    FishAbility CreateAbility(FishType fishType)
    {
        switch (fishType)
        {
            case FishType.Pufferfish:
                return gameObject.AddComponent<PufferfishAbility>();
            case FishType.Shark:
                return gameObject.AddComponent<SharkAbility>();
            case FishType.Clownfish:
                return gameObject.AddComponent<ClownfishAbility>();
            case FishType.Squid:
                return gameObject.AddComponent<SquidAbility>();
            case FishType.Swordfish:
                return gameObject.AddComponent<SwordfishAbility>();
            default:
                return null;
        }
    }

    public FishAbility GetSlot(int index)
    {
        return slots[index];
    }
}