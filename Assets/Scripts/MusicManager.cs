using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [Header("Música de esta escena")]
    public AudioClip[] songs;

    private int currentSongIndex = 0;

    void Start()
    {
        if (songs.Length == 0) return;
        StartCoroutine(WaitAndPlay());
    }

    IEnumerator WaitAndPlay()
    {
        // Esperamos un frame para que todo se inicialice
        yield return null;
        yield return null;

        CinematicManager cm = FindFirstObjectByType<CinematicManager>();

        // Si hay cinemática y no se ha visto esperamos
        if (cm != null && !CinematicManager.hasPlayedCinematic)
        {
            // Silenciamos mientras esperamos
            if (AudioManager.Instance != null)
                AudioManager.Instance.musicSource.volume = 0f;

            yield return new WaitUntil(() => !cm.isPlaying);
        }

        StartCoroutine(PlayPlaylist());
    }

    IEnumerator PlayPlaylist()
    {
        while (true)
        {
            AudioClip song = songs[currentSongIndex];

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMusic(song);

            yield return new WaitForSeconds(song.length);

            currentSongIndex = (currentSongIndex + 1) % songs.Length;
        }
    }
}