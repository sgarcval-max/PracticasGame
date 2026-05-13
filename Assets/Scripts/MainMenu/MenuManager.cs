using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject howToPlayPanel;

    [Header("Buttons")]
    public Button playButton;
    public Button optionsButton;
    public Button quitButton;
    public Button optionsBackButton;

    [Header("Animación de entrada")]
    public float buttonDelay = 0.15f;
    public RectTransform[] buttons;

    void Start()
    {
        // Conectar botones
        playButton.onClick.AddListener(Play);
        optionsButton.onClick.AddListener(OpenOptions);
        quitButton.onClick.AddListener(Quit);
        optionsBackButton.onClick.AddListener(CloseOptions);

        // Panels
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);

        // Animar entrada de botones
        StartCoroutine(AnimateButtonsIn());
    }

    IEnumerator AnimateButtonsIn()
    {
        foreach (RectTransform btn in buttons)
        {
            btn.localScale = Vector3.zero;
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < buttons.Length; i++)
        {
            StartCoroutine(ScaleIn(buttons[i], 0.3f));
            yield return new WaitForSeconds(buttonDelay);
        }
    }

    IEnumerator ScaleIn(RectTransform rt, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            // Efecto de rebote
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
            rt.localScale = Vector3.one * scale;
            yield return null;
        }
        rt.localScale = Vector3.one;
    }

    void Play()
    {
        SceneManager.LoadScene("BaseScene");
    }

    void OpenOptions()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    void CloseOptions()
    {
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    void Quit()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego");
    }
}