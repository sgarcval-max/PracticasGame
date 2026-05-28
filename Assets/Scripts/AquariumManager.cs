using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class AquariumManager : MonoBehaviour
{
    [Header("Acuario")]
    public GameObject aquariumObject;

    [Header("Prefabs de peces del acuario")]
    public GameObject pufferfishPrefab;
    public GameObject sharkPrefab;
    public GameObject clownfishPrefab;
    public GameObject squidPrefab;
    public GameObject swordfishPrefab;
    public GameObject cirujanoPrefa;

    [Header("Panel de info del pez")]
    public GameObject fishInfoPanel;
    public TextMeshProUGUI fishInfoName;
    public TextMeshProUGUI fishInfoDescription;
    public Button equipButton;
    public Button unequipButton;

    private FishType currentHoveredFish;
    private BaseManager baseManager;
    private AquariumFish lastHovered = null;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Awake()
    {
        baseManager = FindFirstObjectByType<BaseManager>();
        fishInfoPanel.SetActive(false);

        if (aquariumObject != null)
        {
            Vector3 size = aquariumObject.transform.localScale;
            Vector3 pos = aquariumObject.transform.position;

            minBounds = new Vector2(pos.x - size.x / 2f + 0.5f, pos.y - size.y / 2f + 0.5f);
            maxBounds = new Vector2(pos.x + size.x / 2f - 0.5f, pos.y + size.y / 2f - 0.5f);
        }

        equipButton.onClick.AddListener(EquipCurrentFish);
        unequipButton.onClick.AddListener(UnequipCurrentFish);
    }

    void Start()
    {
        Debug.Log("AquariumManager iniciado!");
    }

    void Update()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );
        mouseWorld.z = 0f;

        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero);

        if (hit.collider != null)
        {
            AquariumFish fish = hit.collider.GetComponent<AquariumFish>();
            if (fish != null && fish != lastHovered)
            {
                if (lastHovered != null)
                    lastHovered.OnHoverExit();

                lastHovered = fish;
                fish.OnHoverEnter();
                ShowFishInfo(fish.fishType, fish.transform.position);
            }
        }
        else
        {
            if (lastHovered != null && !IsMouseOverInfoPanel())
            {
                lastHovered.OnHoverExit();
                lastHovered = null;
                HideFishInfo();
            }
        }
    }

    bool IsMouseOverInfoPanel()
    {
        if (!fishInfoPanel.activeSelf) return false;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        RectTransform rect = fishInfoPanel.GetComponent<RectTransform>();

        return RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos);
    }

    public void SpawnFish(FishType fishType)
    {
        GameObject prefab = GetPrefabForType(fishType);
        if (prefab == null) return;

        GameObject fish = Instantiate(prefab, transform);
        AquariumFish af = fish.GetComponent<AquariumFish>();
        if (af != null)
            af.Setup(fishType, minBounds, maxBounds);
    }

    GameObject GetPrefabForType(FishType fishType)
    {
        switch (fishType)
        {
            case FishType.Pufferfish: return pufferfishPrefab;
            case FishType.Shark: return sharkPrefab;
            case FishType.Clownfish: return clownfishPrefab;
            case FishType.Squid: return squidPrefab;
            case FishType.Swordfish: return swordfishPrefab;
            case FishType.Cirujano: return cirujanoPrefa;
            default: return null;
        }
    }

    public void ClearFish()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<AquariumFish>() != null)
                Destroy(child.gameObject);
        }
    }

    public void ShowFishInfo(FishType fishType, Vector3 worldPos)
    {
        currentHoveredFish = fishType;

        fishInfoName.text = FishData.GetName(fishType);
        fishInfoDescription.text = FishData.GetDescription(fishType);

        bool isEquipped = GameManager.Instance.equippedFish.Contains(fishType);
        equipButton.gameObject.SetActive(!isEquipped);
        unequipButton.gameObject.SetActive(isEquipped);

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        fishInfoPanel.transform.position = screenPos + new Vector3(120f, 60f, 0f);

        fishInfoPanel.SetActive(true);
    }

    public void HideFishInfo()
    {
        fishInfoPanel.SetActive(false);
    }

    void EquipCurrentFish()
    {
        if (GameManager.Instance.equippedFish.Count >= 3)
        {
            Debug.Log("Ya tienes 3 peces equipados!");
            return;
        }

        if (!GameManager.Instance.equippedFish.Contains(currentHoveredFish))
        {
            GameManager.Instance.equippedFish.Add(currentHoveredFish);
            baseManager?.RefreshUI();
        }

        if (lastHovered != null)
        {
            lastHovered.OnHoverExit();
            lastHovered = null;
        }
        HideFishInfo();
    }

    void UnequipCurrentFish()
    {
        GameManager.Instance.equippedFish.Remove(currentHoveredFish);
        baseManager?.RefreshUI();

        if (lastHovered != null)
        {
            lastHovered.OnHoverExit();
            lastHovered = null;
        }
        HideFishInfo();
    }

    public void RefreshAquarium()
    {
        ClearFish();

        if (GameManager.Instance == null)
        {
            Debug.Log("GameManager es null");
            return;
        }

        Debug.Log("Peces domesticados: " + GameManager.Instance.tamedFish.Count);

        foreach (FishType fish in GameManager.Instance.tamedFish)
        {
            Debug.Log("Spawneando: " + fish);
            SpawnFish(fish);
        }
    }
}