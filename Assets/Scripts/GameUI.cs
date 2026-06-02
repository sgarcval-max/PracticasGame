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
            if (text != null) text.text = FishData.GetName(ability.fishType) + "\n";
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
        StartCoroutine(FadeText(waveText, true, 0.3f));
    }

    public void UpdateTreasure(int collected, int total)
    {
        if (treasureText == null) return;
        treasureText.text = " " + collected + "/" + total;
        if (collected >= total) treasureText.color = new Color(1f, 0.8f, 0f);
    }

    public void UpdateSlots(int equippedCount) { }

    public void ShowWaveComplete(int wave, System.Action onComplete)
    {
        StartCoroutine(WaveCompleteCoroutine(wave, onComplete));
    }

    IEnumerator WaveCompleteCoroutine(int wave, System.Action onComplete)
    {
        yield return StartCoroutine(FadeText(waveText, false, 0.3f));
        waveCompleteText.text = "Oleada " + wave + " completada!";
        waveCompleteText.gameObject.SetActive(true);
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            StartCoroutine(PunchScale(countdownText.transform));
            yield return new WaitForSeconds(1f);
        }

        waveCompleteText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        yield return StartCoroutine(FadeText(waveText, true, 0.3f));
        onComplete?.Invoke();
    }

    IEnumerator FadeText(TextMeshProUGUI text, bool fadeIn, float duration)
    {
        if (text == null) yield break;
        float start = fadeIn ? 0f : 1f;
        float end = fadeIn ? 1f : 0f;
        float timer = 0f;
        Color color = text.color;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; // UnscaledDeltaTime para que la UI no se congele en la victoria
            color.a = Mathf.Lerp(start, end, timer / duration);
            text.color = color;
            yield return null;
        }
        color.a = end;
        text.color = color;
    }

    IEnumerator PunchScale(Transform t)
    {
        Vector3 original = Vector3.one;
        t.localScale = Vector3.one * 1.5f;
        float timer = 0f;
        while (timer < 0.3f)
        {
            timer += Time.unscaledDeltaTime; // UnscaledDeltaTime para que la animación funcione en pausa
            t.localScale = Vector3.Lerp(Vector3.one * 1.5f, original, timer / 0.3f);
            yield return null;
        }
        t.localScale = original;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Pausamos el juego si muere
    }

    // --- AQUÍ CONGELAMOS EL MUNDO PERO EL BUZO SIGUE ALIVIANADO ---
    public void ShowVictory()
    {
        victoryPanel.SetActive(true);

        // 1. Pausamos absolutamente todo el mundo (enemigos, oleadas, temporizadores)
        Time.timeScale = 0f;

        // 2. Quitamos control al buzo. Como su animator pasa a Unscaled en el Awake, se quedará respirando en IDLE.
        DiverController player = FindFirstObjectByType<DiverController>();
        if (player != null)
        {
            player.SetControl(false);
        }
    }

    void Restart()
    {
        Time.timeScale = 1f;
        RestorePlayerControl();
        if (SceneTransition.Instance != null) SceneTransition.Instance.TransitionToGame();
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GoToBase()
    {
        Time.timeScale = 1f;
        RestorePlayerControl();
        if (SceneTransition.Instance != null) SceneTransition.Instance.TransitionToBase();
        else SceneManager.LoadScene("BaseScene");
    }

    private void RestorePlayerControl()
    {
        DiverController player = FindFirstObjectByType<DiverController>();
        if (player != null) player.SetControl(true);
    }
}