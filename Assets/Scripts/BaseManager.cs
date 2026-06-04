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

    [Header("Imágenes de la mochila")]
    public Sprite bagClosedSprite;
    public Sprite bagOpenSprite;
    public Image bagButtonImage;

    [Header("Mochila - Panel y Animación")]
    public GameObject bagPanel;
    public Button openBagButton;
    public Button closeBagButton;
    public float animationSpeed = 8f;
    private Coroutine bagCoroutine;

    [Header("Mochila - Contenido")]
    public Transform bagScrollContent;
    public GameObject fishCardPrefab;

    [Header("Sistema de Equipamiento Físico")]
    public GameObject physicalFishPrefab;
    public Transform spawnPoint;
    private List<GameObject> spawnedFishes = new List<GameObject>();

    [Header("Sprites de los Peces (Asignar en Inspector)")]
    public Sprite spritePufferfish;
    public Sprite spriteShark;
    public Sprite spriteClownfish;
    public Sprite spriteSquid;
    public Sprite spriteSwordfish;
    public Sprite spriteCirujano;

    [Header("Opciones")]
    public GameObject optionsPanel;
    public Button optionsButton;
    public Button optionsBackButton;

    [Header("Navegación")]
    public Button playButton;
    public Button menuButton;

    private TamingMinigame tamingMinigame;

    void Start()
    {
        tamingMinigame = FindFirstObjectByType<TamingMinigame>();

        playButton.onClick.AddListener(GoToSea);
        optionsButton.onClick.AddListener(() => optionsPanel.SetActive(true));
        optionsBackButton.onClick.AddListener(() => optionsPanel.SetActive(false));
        openBagButton.onClick.AddListener(OpenBag);
        closeBagButton.onClick.AddListener(CloseBag);
        menuButton.onClick.AddListener(GoToMenu);

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
        UpdatePhysicalEquippedFishes();
    }

    // --- ANIMACIÓN MOCHILA ---
    public void OpenBag()
    {
        if (bagCoroutine != null) StopCoroutine(bagCoroutine);
        if (AudioManager.Instance != null && AudioManager.Instance.bagOpenSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.bagOpenSound);

        // Cambiamos a imagen de mochila abierta
        if (bagButtonImage != null && bagOpenSprite != null)
            bagButtonImage.sprite = bagOpenSprite;

        bagPanel.SetActive(true);
        RefreshUI();
        bagCoroutine = StartCoroutine(AnimateBag(Vector3.one));
    }

    public void CloseBag()
    {
        if (bagCoroutine != null) StopCoroutine(bagCoroutine);
        if (AudioManager.Instance != null && AudioManager.Instance.bagCloseSound != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.bagCloseSound);

        // Cambiamos a imagen de mochila cerrada
        if (bagButtonImage != null && bagClosedSprite != null)
            bagButtonImage.sprite = bagClosedSprite;

        bagCoroutine = StartCoroutine(AnimateBag(Vector3.zero, () => bagPanel.SetActive(false)));
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

    // --- LÓGICA FÍSICA ---
    void UpdatePhysicalEquippedFishes()
    {
        foreach (GameObject fishObj in spawnedFishes) Destroy(fishObj);
        spawnedFishes.Clear();

        List<FishType> equipped = GameManager.Instance.equippedFish;
        for (int i = 0; i < equipped.Count; i++)
        {
            SpawnFishInWorld(equipped[i], i);
        }
    }

    void SpawnFishInWorld(FishType type, int index)
    {
        if (physicalFishPrefab == null || spawnPoint == null) return;

        Vector3 spawnOffset = new Vector3(index * 0.4f, 0f, 0f);
        GameObject newFish = Instantiate(physicalFishPrefab, spawnPoint.position + spawnOffset, Quaternion.identity);
        newFish.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        PhysicalEquippedFish fishScript = newFish.GetComponent<PhysicalEquippedFish>();
        if (fishScript != null)
        {
            fishScript.Setup(type, GetSpriteForType(type));
        }
        spawnedFishes.Add(newFish);
    }

    Sprite GetSpriteForType(FishType type)
    {
        switch (type)
        {
            case FishType.Pufferfish: return spritePufferfish;
            case FishType.Shark: return spriteShark;
            case FishType.Clownfish: return spriteClownfish;
            case FishType.Squid: return spriteSquid;
            case FishType.Swordfish: return spriteSwordfish;
            case FishType.Cirujano: return spriteCirujano;
            default: return spriteClownfish;
        }
    }

    // --- MISIONES Y BAG ---
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
        foreach (Transform child in bagScrollContent) Destroy(child.gameObject);

        Dictionary<FishType, int> fishCount = new Dictionary<FishType, int>();
        foreach (FishType fish in GameManager.Instance.caughtFish)
        {
            if (fishCount.ContainsKey(fish)) fishCount[fish]++;
            else fishCount[fish] = 1;
        }

        List<FishType> unique = GetUniqueCaughtFish();
        for (int i = 0; i < unique.Count; i++)
        {
            GameObject card = Instantiate(fishCardPrefab, bagScrollContent);
            card.GetComponent<FishCard>().SetupBagCard(unique[i], fishCount[unique[i]], i, this);
        }
    }

    void UpdateAquarium()
    {
        FindFirstObjectByType<AquariumManager>()?.RefreshAquarium();
    }

    public void StartTaming(int fishIndex)
    {
        List<FishType> uniqueFish = GetUniqueCaughtFish();
        if (fishIndex >= uniqueFish.Count) return;
        FishType fish = uniqueFish[fishIndex];
        tamingMinigame?.StartMinigame(fish, (success) => {
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
            if (!unique.Contains(fish)) unique.Add(fish);
        return unique;
    }

    List<FishType> GetUniqueTamedFish()
    {
        List<FishType> unique = new List<FishType>();
        foreach (FishType fish in GameManager.Instance.tamedFish)
            if (!unique.Contains(fish)) unique.Add(fish);
        return unique;
    }

    void GoToSea()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionToGame();
        else
            SceneManager.LoadScene("GameScene");
    }

    void GoToMenu()
    {
        // Marcamos que venimos de la base
        PlayerPrefs.SetInt("ComingFromBase", 1);
        PlayerPrefs.Save();

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.TransitionToMenuWithFade();
        else
            SceneManager.LoadScene("MainMenu");
    }
}