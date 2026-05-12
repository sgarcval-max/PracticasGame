using UnityEngine;
using UnityEngine.InputSystem;

public class DiverController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float maxSpeed = 12f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Este método lo llama automáticamente el componente PlayerInput
    // cuando detecta movimiento en WASD o flechas
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // Voltear sprite según dirección horizontal
        if (moveInput.x > 0) sr.flipX = false;
        if (moveInput.x < 0) sr.flipX = true;
    }

    void FixedUpdate()
    {
        // Empujar el rigidbody en la dirección del input
        rb.AddForce(moveInput * moveSpeed, ForceMode2D.Force);

        // Limitar velocidad máxima
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}