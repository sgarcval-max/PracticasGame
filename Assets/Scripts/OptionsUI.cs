using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider cinematicSlider;

    [Header("Textos de porcentaje")]
    public TextMeshProUGUI masterText;
    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;
    public TextMeshProUGUI cinematicText;

    void Start()
    {
        // Cargamos los valores guardados
        if (AudioManager.Instance != null)
        {
            masterSlider.value = AudioManager.Instance.masterVolume;
            musicSlider.value = AudioManager.Instance.musicVolume;
            sfxSlider.value = AudioManager.Instance.sfxVolume;
            cinematicSlider.value = AudioManager.Instance.cinematicVolume;
        }

        UpdateTexts();

        // Conectamos los sliders
        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        cinematicSlider.onValueChanged.AddListener(OnCinematicChanged);
    }

    void OnMasterChanged(float value)
    {
        AudioManager.Instance?.SetMasterVolume(value);
        masterText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    void OnMusicChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        musicText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    void OnSFXChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
        sfxText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    void OnCinematicChanged(float value)
    {
        AudioManager.Instance?.SetCinematicVolume(value);
        cinematicText.text = Mathf.RoundToInt(value * 100) + "%";

        // Actualizamos el volumen del VideoPlayer si hay una cinemática activa
        CinematicManager cm = FindFirstObjectByType<CinematicManager>();
        if (cm != null && cm.videoPlayer != null)
        {
            float vol = AudioManager.Instance.masterVolume * value;
            cm.videoPlayer.SetDirectAudioVolume(0, vol);
        }
    }

    void UpdateTexts()
    {
        masterText.text = Mathf.RoundToInt(masterSlider.value * 100) + "%";
        musicText.text = Mathf.RoundToInt(musicSlider.value * 100) + "%";
        sfxText.text = Mathf.RoundToInt(sfxSlider.value * 100) + "%";
        cinematicText.text = Mathf.RoundToInt(cinematicSlider.value * 100) + "%";
    }
}
