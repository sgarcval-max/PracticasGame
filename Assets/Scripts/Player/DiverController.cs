using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class DiverController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 8f;
    public float maxSpeed = 12f;
    public float linearDrag = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.linearDamping = linearDrag;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // Voltear sprite según dirección horizontal
        if (moveInput.x > 0) sr.flipX = false;
        if (moveInput.x < 0) sr.flipX = true;

        // Detectar si está presionando teclas
        bool isPressingKeys = moveInput.magnitude > 0.1f;

        // Actualizar Animator
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        animator.SetBool("IsMoving", isPressingKeys);
        animator.SetFloat("VerticalSpeed", moveInput.y);
    }

    void FixedUpdate()
    {
        rb.AddForce(moveInput * moveSpeed, ForceMode2D.Force);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    public void TriggerDeath()
    {
        animator.SetTrigger("Die");
    }
}