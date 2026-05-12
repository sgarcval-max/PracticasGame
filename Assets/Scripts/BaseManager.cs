using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BaseManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI missionText;
    public TextMeshProUGUI fishCountText;
    public Button playButton;

    void Start()
    {
        playButton.onClick.AddListener(GoToSea);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (GameManager.Instance == null)
        {
            Debug.Log("GameManager no encontrado");
            return;
        }

        UpdateMission();
        UpdateFishCount();
    }

    void UpdateMission()
    {
        if (missionText == null) return;

        if (GameManager.Instance.missionCompleted)
        {
            missionText.text = "Misión:\n" + GameManager.Instance.currentMission + "\n\n✓ COMPLETADA!";
            missionText.color = new Color(0.2f, 0.8f, 0.2f);
        }
        else
        {
            missionText.text = "Misión:\n" + GameManager.Instance.currentMission + "\n\nPendiente...";
            missionText.color = Color.white;
        }
    }

    void UpdateFishCount()
    {
        if (fishCountText == null) return;

        int total = GameManager.Instance.allFish.Count;
        int equipped = GameManager.Instance.equippedFish.Count;

        fishCountText.text = "Peces capturados: " + total +
                             "\nEquipados: " + equipped + "/3" +
                             "\nOleada actual: " + GameManager.Instance.currentWave;
    }

    void GoToSea()
    {
        SceneManager.LoadScene("GameScene");
    }
}
