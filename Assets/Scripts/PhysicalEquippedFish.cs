using UnityEngine;

public class PhysicalEquippedFish : MonoBehaviour
{
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    public void Setup(FishType type, Sprite fishSprite)
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // 1. Ponemos la imagen del pez
        if (sr != null) sr.sprite = fishSprite;

        // 2. Ajustamos el tamaño (Escala)
        // Cámbialo aquí si salen muy grandes o pequeños
        transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        // 3. Empujoncito para que caiga con estilo
        if (rb != null)
        {
            rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
        }
        
        // 4. Ajuste de colisión automático
        // Esto hace que el collider se adapte al tamaño de la imagen nueva
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null && fishSprite != null)
        {
            col.size = sr.sprite.bounds.size;
        }
    }
}
