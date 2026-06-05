using UnityEngine;
using System.Collections;

public class ClownfishAbility : FishAbility
{
    public float decoyDuration = 5f;
    public GameObject decoyPrefab;

    void Awake()
    {
        abilityName = "Señuelo";
        cooldown = 10f;
        duration = decoyDuration;
        fishType = FishType.Clownfish;
    }

    protected override void Activate()
    {
        Debug.Log("Señuelo activado!");
        StartCoroutine(DecoyCoroutine());
    }

    IEnumerator DecoyCoroutine()
    {
        Vector3 decoyPos = transform.position + new Vector3(
            Random.Range(-3f, 3f),
            Random.Range(-3f, 3f),
            0f
        );

        GameObject decoy = null;

        if (decoyPrefab != null)
        {
            decoy = Instantiate(decoyPrefab, decoyPos, Quaternion.identity);
        }
        else
        {
            decoy = new GameObject("Decoy");
            decoy.transform.position = decoyPos;
            SpriteRenderer sr = decoy.AddComponent<SpriteRenderer>();
            sr.color = new Color(1f, 0.5f, 0f);
        }

        // Añadimos el script de señuelo
        DecoyTarget dt = decoy.AddComponent<DecoyTarget>();

        // Efecto de parpadeo cuando queda poco tiempo
        StartCoroutine(BlinkDecoy(decoy, decoyDuration));

        yield return new WaitForSeconds(decoyDuration);

        if (decoy != null)
            Destroy(decoy);

        Debug.Log("Señuelo terminado!");
    }

    IEnumerator BlinkDecoy(GameObject decoy, float duration)
    {
        if (decoy == null) yield break;

        SpriteRenderer sr = decoy.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            // Parpadeo cuando queda menos de 2 segundos
            if (timer > duration - 2f)
                sr.enabled = Mathf.Sin(timer * 10f) > 0;

            if (decoy == null) yield break;
            yield return null;
        }
    }
}

