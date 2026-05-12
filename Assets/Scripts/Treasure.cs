using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int value = 1;

    private SpriteRenderer sr;
    private float bobSpeed = 2f;
    private float bobAmount = 0.1f;
    private Vector3 startPos;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Tocando: " + other.gameObject.name + " Tag: " + other.gameObject.tag);

        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        TreasureManager tm = FindFirstObjectByType<TreasureManager>();
        if (tm != null)
        {
            tm.CollectTreasure(value);
        }

        Destroy(gameObject);
    }
}
