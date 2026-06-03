using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialPopUpManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject panelPopUp;
    public TextMeshProUGUI textoCuerpo;
    public Button botonCerrar;

    void Awake()
    {
        panelPopUp.SetActive(false);
        botonCerrar.onClick.AddListener(CerrarMensaje);
    }

    public void MostrarTutorial(string idMecanica, string mensaje)
    {
        // Solo muestra el mensaje si no se ha visto en esta sesión
        if (PlayerPrefs.GetInt("Tutorial_" + idMecanica, 0) == 0)
        {
            textoCuerpo.text = mensaje;
            panelPopUp.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego
            PlayerPrefs.SetInt("Tutorial_" + idMecanica, 1);
            PlayerPrefs.Save();
        }
    }

    public void CerrarMensaje()
    {
        panelPopUp.SetActive(false);
        Time.timeScale = 1f; // Reanuda el juego
    }
}