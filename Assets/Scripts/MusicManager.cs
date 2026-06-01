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
        StartCoroutine(PlayPlaylist());
    }

    IEnumerator PlayPlaylist()
    {
        while (true)
        {
            AudioClip song = songs[currentSongIndex];

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayMusic(song);

            // Esperamos a que termine la canción
            yield return new WaitForSeconds(song.length);

            // Siguiente canción
            currentSongIndex = (currentSongIndex + 1) % songs.Length;
        }
    }
}
