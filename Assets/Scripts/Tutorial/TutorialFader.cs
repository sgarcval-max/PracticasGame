using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialFader : MonoBehaviour
{
    public void StartFade(string sceneName)
    {
        StartCoroutine(DoFade(sceneName));
    }

    IEnumerator DoFade(string sceneName)
    {
        // Crear canvas
        GameObject fadeCanvas = new GameObject("FadeCanvas");
        fadeCanvas.transform.SetParent(transform);
        Canvas fc = fadeCanvas.AddComponent<Canvas>();
        fc.renderMode = RenderMode.ScreenSpaceOverlay;
        fc.sortingOrder = 9999;
        fadeCanvas.AddComponent<CanvasScaler>();

        // Imagen negra
        GameObject blackObj = new GameObject("BlackScreen");
        blackObj.transform.SetParent(fadeCanvas.transform, false);
        Image blackImage = blackObj.AddComponent<Image>();
        blackImage.color = new Color(0, 0, 0, 0);

        RectTransform rt = blackObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Fade in negro
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, timer / 0.5f);
            yield return null;
        }
        blackImage.color = Color.black;

        // Cargamos la escena
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSecondsRealtime(0.3f);
        asyncLoad.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncLoad.isDone);

        yield return new WaitForEndOfFrame();

        // Fade out negro
        timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.unscaledDeltaTime;
            blackImage.color = new Color(0, 0, 0, 1 - timer / 0.5f);
            yield return null;
        }

        // Destruimos el objeto entero
        Destroy(gameObject);
    }
}
