using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    [Header("Lista de canciones para esta escena")]
    public AudioClip[] songs;

    private int currentSongIndex = 0;
    private Coroutine playlistCoroutine;

    void Start()
    {
        if (songs.Length == 0)
        {
            Debug.LogWarning("MusicManager: No has puesto canciones en la lista de este objeto.");
            return;
        }

        // Si por algún motivo ya había una rutina, la limpiamos
        if (playlistCoroutine != null) StopCoroutine(playlistCoroutine);

        playlistCoroutine = StartCoroutine(PlayPlaylist());
    }

    IEnumerator PlayPlaylist()
    {
        while (true)
        {
            AudioClip song = songs[currentSongIndex];

            if (song != null && AudioManager.Instance != null)
            {
                Debug.Log("MusicManager: Solicitando canción " + currentSongIndex + ": " + song.name);
                AudioManager.Instance.PlayMusic(song);

                // Esperamos la duración del clip usando tiempo real (ignora pausas)
                // Le restamos 0.1s para asegurar que la transición sea fluida
                float waitTime = song.length > 0.1f ? song.length - 0.1f : 0.1f;
                yield return new WaitForSecondsRealtime(waitTime);
            }
            else
            {
                // Si hay un error, espera 1 segundo y reintenta
                yield return new WaitForSecondsRealtime(1f);
            }

            // Pasamos a la siguiente canción (Ciclo infinito)
            currentSongIndex = (currentSongIndex + 1) % songs.Length;
        }
    }

    private void OnDisable()
    {
        // Si el objeto se destruye al cambiar de escena, paramos la rutina
        if (playlistCoroutine != null) StopCoroutine(playlistCoroutine);
    }
}