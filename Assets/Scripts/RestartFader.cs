using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RestartFader : MonoBehaviour
{
    public Image blackImage;

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForEndOfFrame();

        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            if (blackImage != null)
                blackImage.color = new Color(0, 0, 0, 1 - timer / 0.5f);
            yield return null;
        }

        Destroy(gameObject);
    }
}
