using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class BaseCinematicManager : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public RawImage videoImage;
    public GameObject cinematicCanvas;

    [Header("Base")]
    public GameObject baseCanvas;

    void Start()
    {
        // Leemos de PlayerPrefs en lugar del GameManager
        bool comingFromMenu = PlayerPrefs.GetInt("ComingFromMenu", 0) == 1;

        Debug.Log("ComingFromMenu: " + comingFromMenu);

        if (comingFromMenu)
        {
            // Reseteamos el valor
            PlayerPrefs.SetInt("ComingFromMenu", 0);
            PlayerPrefs.Save();

            baseCanvas.SetActive(false);
            cinematicCanvas.SetActive(true);
            StartCoroutine(PlayCinematic());
        }
        else
        {
            cinematicCanvas.SetActive(false);
            baseCanvas.SetActive(true);
        }
    }

    IEnumerator PlayCinematic()
    {
        videoPlayer.Prepare();
        yield return new WaitUntil(() => videoPlayer.isPrepared);
        videoPlayer.Play();

        // Esperamos a que termine el video
        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        EndCinematic();
    }

    void EndCinematic()
    {
        // Desactivamos la cinemática y activamos la base
        cinematicCanvas.SetActive(false);
        videoPlayer.gameObject.SetActive(false);
        baseCanvas.SetActive(true);
    }
}
