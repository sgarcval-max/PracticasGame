using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Vida")]
    public Slider healthBar;

    [Header("Oleada")]
    public TextMeshProUGUI waveText;

    [Header("Tesoro")]
    public TextMeshProUGUI treasureText;

    [Header("Slots de habilidades")]
    public Image slot1;
    public Image slot2;
    public Image slot3;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button baseButton;

    [Header("Victoria")]
    public GameObject victoryPanel;
    public Button victoryBaseButton;

    private Color slotEmpty = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    private Color slotReady = new Color(0.2f, 0.8f, 0.4f, 1f);
    private Color slotCooldown = new Color(0.8f, 0.2f, 0.2f, 1f);

    private AbilityManager abilityManager;

    void Awake()
    {
        abilityManager = FindFirstObjectByType<AbilityManager>();
    }

    void Start()
    {
        UpdateSlots(0);

        // Conectar botones Game Over
        restartButton.onClick.AddListener(Restart);
        baseButton.onClick.AddListener(GoToBase);

        // Conectar botón Victoria
        victoryBaseButton.onClick.AddListener(GoToBase);

        // Aseguramos que los paneles están ocultos
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
    }

    void Update()
    {
        RefreshSlotColors();
    }

    void RefreshSlotColors()
    {
        if (abilityManager == null) return;

        UpdateSlotColor(slot1, abilityManager.GetSlot(0));
        UpdateSlotColor(slot2, abilityManager.GetSlot(1));
        UpdateSlotColor(slot3, abilityManager.GetSlot(2));
    }

    void UpdateSlotColor(Image slot, FishAbility ability)
    {
        if (slot == null) return;

        if (ability == null)
            slot.color = slotEmpty;
        else if (ability.IsReady())
            slot.color = slotReady;
        else
            slot.color = slotCooldown;
    }

    public void UpdateHealth(int current, int max)
    {
        if (healthBar == null) return;
        healthBar.maxValue = max;
        healthBar.value = current;
    }

    public void UpdateWave(int wave)
    {
        if (waveText == null) return;
        waveText.text = "Oleada " + wave;
    }

    public void UpdateTreasure(int collected, int total)
    {
        if (treasureText == null) return;
        treasureText.text = "💎 " + collected + "/" + total;

        if (collected >= total)
            treasureText.color = new Color(1f, 0.8f, 0f);
    }

    public void UpdateSlots(int equippedCount)
    {
        slot1.color = equippedCount >= 1 ? slotReady : slotEmpty;
        slot2.color = equippedCount >= 2 ? slotReady : slotEmpty;
        slot3.color = equippedCount >= 3 ? slotReady : slotEmpty;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void ShowVictory()
    {
        victoryPanel.SetActive(true);
    }

    void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }

    void GoToBase()
    {
        SceneManager.LoadScene("BaseScene");
    }
}