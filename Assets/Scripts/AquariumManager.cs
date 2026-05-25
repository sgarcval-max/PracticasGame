using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class AquariumManager : MonoBehaviour
{
    [Header("Acuario")]
    public GameObject aquariumObject;
    public GameObject fishPrefab;

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
        try
        {
            Debug.Log("Update ejecutandose");

            if (Camera.main == null) return;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
       UnityEngine.InputSystem.Mouse.current.position.ReadValue()
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
                // Solo ocultamos el panel si el ratón no está encima del panel de info
                if (lastHovered != null && !IsMouseOverInfoPanel())
                {
                    lastHovered.OnHoverExit();
                    lastHovered = null;
                    HideFishInfo();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error en Update: " + e.Message);
        }
    }

    bool IsMouseOverInfoPanel()
    {
        if (!fishInfoPanel.activeSelf) return false;

        Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        RectTransform rect = fishInfoPanel.GetComponent<RectTransform>();

        return RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos);
    }

    public void SpawnFish(FishType fishType)
    {
        if (fishPrefab == null) return;

        GameObject fish = Instantiate(fishPrefab, transform);
        AquariumFish af = fish.GetComponent<AquariumFish>();
        af.Setup(fishType, minBounds, maxBounds);
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
    }

    void UnequipCurrentFish()
    {
        GameManager.Instance.equippedFish.Remove(currentHoveredFish);
        baseManager?.RefreshUI();
        HideFishInfo();
    }

    public void RefreshAquarium()
    {
        ClearFish();

        if (GameManager.Instance == null) return;

        foreach (FishType fish in GameManager.Instance.tamedFish)
        {
            SpawnFish(fish);
        }
    }
}