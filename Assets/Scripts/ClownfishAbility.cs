using UnityEngine;

public class ClownfishAbility : FishAbility
{
    public float decoyDuration = 5f;

    void Awake()
    {
        abilityName = "Señuelo";
        cooldown = 10f;
        duration = decoyDuration;
        fishType = FishType.Pufferfish; // <- añade esta línea
    }

    protected override void Activate()
    {
        Debug.Log("Señuelo activado!");
        StartCoroutine(DecoyCoroutine());
    }

    System.Collections.IEnumerator DecoyCoroutine()
    {
        // Creamos un señuelo en una posición aleatoria cerca del buzo
        Vector3 decoyPos = transform.position + new Vector3(
            Random.Range(-3f, 3f),
            Random.Range(-3f, 3f),
            0f
        );

        // Creamos un objeto vacío como señuelo
        GameObject decoy = new GameObject("Decoy");
        decoy.transform.position = decoyPos;

        // Le añadimos un sprite para verlo
        SpriteRenderer sr = decoy.AddComponent<SpriteRenderer>();
        sr.color = new Color(1f, 0.5f, 0f);

        // Añadimos el script de señuelo
        DecoyTarget dt = decoy.AddComponent<DecoyTarget>();

        // Esperamos la duración
        yield return new WaitForSeconds(decoyDuration);

        // Destruimos el señuelo
        if (decoy != null)
            Destroy(decoy);

        Debug.Log("Señuelo terminado!");
    }
}
