using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource cinematicSource;
    public AudioSource countdownSource;

    [Header("Volúmenes iniciales")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float cinematicVolume = 1f;

    [Header("SFX Botones")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;

    [Header("SFX Mochila")]
    public AudioClip bagOpenSound;
    public AudioClip bagCloseSound;

    [Header("SFX Paneles")]
    public AudioClip victorySound;
    public AudioClip gameOverSound;

    [Header("SFX Cuenta Regresiva")]
    public AudioClip countdownSound;

    [Header("SFX Tesoro")]
    public AudioClip treasureSound;

    [Header("SFX Daño")]
    public AudioClip damageSound1;
    public AudioClip damageSound2;

    public bool isCinematicPlaying = false;

    void Start()
    {
        LoadVolumes();
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        if (countdownSource == null)
        {
            countdownSource = gameObject.AddComponent<AudioSource>();
            countdownSource.playOnAwake = false;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada: Refrescando volúmenes...");
        LoadVolumes();
    }

    void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        cinematicVolume = PlayerPrefs.GetFloat("CinematicVolume", 1f);

        Debug.Log($"Volúmenes cargados - Master: {masterVolume}, SFX: {sfxVolume}");

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
        if (musicSource != null && !isCinematicPlaying)
            musicSource.volume = masterVolume * musicVolume;
        else if (isCinematicPlaying)
            musicSource.volume = 0f;

        if (sfxSource != null)
            sfxSource.volume = masterVolume * sfxVolume; // Corregido para que aplique el volumen real de SFX

        if (cinematicSource != null)
            cinematicSource.volume = masterVolume * cinematicVolume;

        if (countdownSource != null && countdownSource.isPlaying)
            countdownSource.volume = masterVolume * sfxVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.volume = masterVolume * sfxVolume;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        // ELIMINA cualquier línea que diga "if (musicSource.clip == clip) return;"

        musicSource.Stop(); // Detenemos la canción actual SI O SI
        musicSource.clip = clip;
        musicSource.Play();

        Debug.Log("AudioManager: Reproduciendo " + clip.name);
    }

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

    public void PlayMusicSFX(AudioClip clip, bool loop = false)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.clip = clip;
        sfxSource.loop = loop;
        sfxSource.volume = masterVolume * musicVolume;
        sfxSource.Play();
    }
}
