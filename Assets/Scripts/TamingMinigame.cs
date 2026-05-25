using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TamingMinigame : MonoBehaviour
{
    [Header("UI")]
    public GameObject minigamePanel;
    public Image indicatorBar;
    public Image greenZone;
    public Image indicator;
    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI resultText;
    public Button closeButton;

    [Header("Configuración")]
    public float indicatorSpeed = 2f;
    public float greenZoneSize = 0.25f;

    private float indicatorPos = 0f;
    private float direction = 1f;
    private bool isPlaying = false;
    private FishType currentFish;
    private Action<bool> onResult;

    private BaseManager baseManager;

    void Awake()
    {
        baseManager = FindFirstObjectByType<BaseManager>();
        minigamePanel.SetActive(false);
        closeButton.onClick.AddListener(CloseMinigame);
    }

    void Update()
    {
        if (!isPlaying) return;

        // Mover el indicador de lado a lado
        indicatorPos += direction * indicatorSpeed * Time.deltaTime;

        if (indicatorPos >= 1f)
        {
            indicatorPos = 1f;
            direction = -1f;
        }
        else if (indicatorPos <= 0f)
        {
            indicatorPos = 0f;
            direction = 1f;
        }

        // Actualizar posición visual del indicador
        indicator.rectTransform.anchorMin = new Vector2(indicatorPos, 0f);
        indicator.rectTransform.anchorMax = new Vector2(indicatorPos, 1f);
        indicator.rectTransform.anchoredPosition = Vector2.zero;

        // Pulsar espacio para intentar domesticar
        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CheckResult();
        }
    }

    public void StartMinigame(FishType fishType, Action<bool> callback)
    {
        currentFish = fishType;
        onResult = callback;
        isPlaying = true;
        indicatorPos = 0f;
        direction = 1f;

        // Nombre del pez
        fishNameText.text = "Domestica: " + FishData.GetName(fishType);
        resultText.text = "Pulsa ESPACIO en la zona verde!";
        resultText.color = Color.white;

        // Posición y tamaño de la zona verde (aleatoria cada vez)
        float greenStart = UnityEngine.Random.Range(0.1f, 1f - greenZoneSize - 0.1f);
        greenZone.rectTransform.anchorMin = new Vector2(greenStart, 0f);
        greenZone.rectTransform.anchorMax = new Vector2(greenStart + greenZoneSize, 1f);
        greenZone.rectTransform.anchoredPosition = Vector2.zero;
        greenZone.rectTransform.sizeDelta = Vector2.zero;

        minigamePanel.SetActive(true);
    }

    void CheckResult()
    {
        float greenStart = greenZone.rectTransform.anchorMin.x;
        float greenEnd = greenZone.rectTransform.anchorMax.x;

        bool success = indicatorPos >= greenStart && indicatorPos <= greenEnd;

        isPlaying = false;

        if (success)
        {
            resultText.text = "✅ Domesticado!";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "❌ Has fallado! Pez perdido.";
            resultText.color = Color.red;
        }

        // Esperamos un momento antes de cerrar
        StartCoroutine(CloseAfterDelay(success));
    }

    System.Collections.IEnumerator CloseAfterDelay(bool success)
    {
        yield return new WaitForSeconds(1.5f);
        onResult?.Invoke(success);
        minigamePanel.SetActive(false);
        baseManager?.RefreshUI();
    }

    void CloseMinigame()
    {
        isPlaying = false;
        minigamePanel.SetActive(false);
    }
}
