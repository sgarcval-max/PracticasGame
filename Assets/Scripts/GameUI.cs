using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

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
    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public TextMeshProUGUI slot3Text;

    [Header("Wave Complete")]
    public TextMeshProUGUI waveCompleteText;
    public TextMeshProUGUI countdownText;

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

        restartButton.onClick.AddListener(Restart);
        baseButton.onClick.AddListener(GoToBase);
        victoryBaseButton.onClick.AddListener(GoToBase);

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

        UpdateSlotColor(slot1, slot1Text, abilityManager.GetSlot(0));
        UpdateSlotColor(slot2, slot2Text, abilityManager.GetSlot(1));
        UpdateSlotColor(slot3, slot3Text, abilityManager.GetSlot(2));
    }

    void UpdateSlotColor(Image slot, TextMeshProUGUI text, FishAbility ability)
    {
        if (slot == null) return;

        if (ability == null)
        {
            slot.color = slotEmpty;
            if (text != null) text.text = "Vacío";
        }
        else if (ability.IsReady())
        {
            slot.color = FishData.GetColor(ability.fishType);
            if (text != null) text.text = FishData.GetName(ability.fishType);
        }
        else
        {
            slot.color = slotCooldown;
            if (text != null) text.text = FishData.GetName(ability.fishType) + "\n⏳";
        }
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

    public void UpdateSlots(int equippedCount) { }

    // Muestra el mensaje de oleada completada y el contador
    public void ShowWaveComplete(int wave, System.Action onComplete)
    {
        StartCoroutine(WaveCompleteCoroutine(wave, onComplete));
    }

    IEnumerator WaveCompleteCoroutine(int wave, System.Action onComplete)
    {
        // Mostrar texto de oleada completada
        waveCompleteText.text = "Oleada " + wave + " completada!";
        waveCompleteText.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(true);

        // Contador 3, 2, 1
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();

            // Animación de escala del número
            StartCoroutine(PunchScale(countdownText.transform));

            yield return new WaitForSeconds(1f);
        }

        // Ocultar textos
        waveCompleteText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);

        // Llamar al callback para iniciar la siguiente oleada
        onComplete?.Invoke();
    }

    IEnumerator PunchScale(Transform t)
    {
        Vector3 original = Vector3.one;
        t.localScale = Vector3.one * 1.5f;

        float timer = 0f;
        while (timer < 0.3f)
        {
            timer += Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.one * 1.5f, original, timer / 0.3f);
            yield return null;
        }

        t.localScale = original;
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