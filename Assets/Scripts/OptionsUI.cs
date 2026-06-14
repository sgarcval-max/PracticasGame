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

    [Header("Sonido del slider")]
    public AudioClip sliderTickSound;

    private int lastMasterTick;
    private int lastMusicTick;
    private int lastSFXTick;
    private int lastCinematicTick;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            masterSlider.value = AudioManager.Instance.masterVolume;
            musicSlider.value = AudioManager.Instance.musicVolume;
            sfxSlider.value = AudioManager.Instance.sfxVolume;
            cinematicSlider.value = AudioManager.Instance.cinematicVolume;
        }

        lastMasterTick = GetTick(masterSlider.value);
        lastMusicTick = GetTick(musicSlider.value);
        lastSFXTick = GetTick(sfxSlider.value);
        lastCinematicTick = GetTick(cinematicSlider.value);

        UpdateTexts();

        masterSlider.onValueChanged.AddListener(OnMasterChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        cinematicSlider.onValueChanged.AddListener(OnCinematicChanged);
    }

    int GetTick(float value)
    {
        return Mathf.FloorToInt(value * 10);
    }

    void PlayTickIfNeeded(float value, ref int lastTick)
    {
        int currentTick = GetTick(value);
        if (currentTick != lastTick)
        {
            lastTick = currentTick;
            PlayTickSound();
        }
    }

    void PlayTickSound()
    {
        if (AudioManager.Instance == null) return;
        AudioClip clip = sliderTickSound != null ? sliderTickSound : AudioManager.Instance.buttonClickSound;
        if (clip != null)
            AudioManager.Instance.PlaySFX(clip);
    }

    void OnMasterChanged(float value)
    {
        AudioManager.Instance?.SetMasterVolume(value);
        masterText.text = Mathf.RoundToInt(value * 100) + "%";
        PlayTickIfNeeded(value, ref lastMasterTick);
    }

    void OnMusicChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
        musicText.text = Mathf.RoundToInt(value * 100) + "%";
        PlayTickIfNeeded(value, ref lastMusicTick);
    }

    void OnSFXChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
        sfxText.text = Mathf.RoundToInt(value * 100) + "%";
        PlayTickIfNeeded(value, ref lastSFXTick);
    }

    void OnCinematicChanged(float value)
    {
        AudioManager.Instance?.SetCinematicVolume(value);
        cinematicText.text = Mathf.RoundToInt(value * 100) + "%";
        PlayTickIfNeeded(value, ref lastCinematicTick);

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
