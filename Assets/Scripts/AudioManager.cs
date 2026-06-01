using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource cinematicSource;

    [Header("Volúmenes iniciales")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float cinematicVolume = 1f;

    [Header("SFX Botones")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cargamos volúmenes guardados
        LoadVolumes();
    }

    void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        cinematicVolume = PlayerPrefs.GetFloat("CinematicVolume", 1f);

        ApplyVolumes();
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        ApplyVolumes();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
        ApplyVolumes();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
        ApplyVolumes();
    }

    public void SetCinematicVolume(float value)
    {
        cinematicVolume = value;
        PlayerPrefs.SetFloat("CinematicVolume", value);
        ApplyVolumes();
    }

    public void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = masterVolume * musicVolume;

        if (sfxSource != null)
            sfxSource.volume = masterVolume * sfxVolume;

        if (cinematicSource != null)
            cinematicSource.volume = masterVolume * cinematicVolume;
    }

    // Reproducir efecto de sonido
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, masterVolume * sfxVolume);
    }

    // Reproducir música
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Parar música
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = musicSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }

    public IEnumerator FadeInMusic(float duration)
    {
        float targetVolume = masterVolume * musicVolume;
        float timer = 0f;
        musicSource.volume = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, timer / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }
}
