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
        playButton.onClick.AddListener(Play);
        optionsButton.onClick.AddListener(OpenOptions);
        quitButton.onClick.AddListener(Quit);
        optionsBackButton.onClick.AddListener(CloseOptions);

        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);

        StartCoroutine(AnimateButtonsIn());
    }

    IEnumerator AnimateButtonsIn()
    {
        // Desactivamos ButtonAnimator y ocultamos botones
        foreach (RectTransform btn in buttons)
        {
            btn.localScale = Vector3.zero;
            ButtonAnimator ba = btn.GetComponent<ButtonAnimator>();
            if (ba != null) ba.enabled = false;
        }

        // Esperamos a que el título termine su animación
        yield return new WaitForSeconds(2f);

        // Animamos todos los botones casi a la vez
        foreach (RectTransform btn in buttons)
        {
            StartCoroutine(ScaleIn(btn, 0.3f));
            yield return new WaitForSeconds(0.05f); // Pequeño delay entre cada uno
        }

        // Esperamos a que terminen todos
        yield return new WaitForSeconds(0.3f);

        // Activamos ButtonAnimator
        foreach (RectTransform btn in buttons)
        {
            ButtonAnimator ba = btn.GetComponent<ButtonAnimator>();
            if (ba != null) ba.enabled = true;
        }
    }

    IEnumerator ScaleIn(RectTransform rt, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            // Efecto de rebote suave
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.1f;
            if (t < 0.6f)
                scale = Mathf.Lerp(0f, 1.1f, t / 0.6f);
            else
                scale = Mathf.Lerp(1.1f, 1f, (t - 0.6f) / 0.4f);

            rt.localScale = Vector3.one * scale;
            yield return null;
        }
        rt.localScale = Vector3.one;
    }

    void Play()
    {
        // Guardamos en PlayerPrefs para que persista entre escenas
        PlayerPrefs.SetInt("ComingFromMenu", 1);
        PlayerPrefs.Save();
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