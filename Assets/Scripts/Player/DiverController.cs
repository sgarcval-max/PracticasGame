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

    // Controla si el jugador puede manejar al buzo
    private bool canControl = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.linearDamping = linearDrag;

        // FUERZA AL ANIMATOR A IGNORAR EL TIMESCALE = 0
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    // Input System: Solo registra movimiento si canControl es true
    void OnMove(InputValue value)
    {
        if (!canControl)
        {
            moveInput = Vector2.zero;
            return;
        }
        moveInput = value.Get<Vector2>();
    }

    void Update()
    {
        // Si no hay control (Victoria), forzamos parámetros de IDLE
        // Al estar en Unscaled Time en el Awake, esto se verá animado aunque el tiempo sea 0
        if (!canControl)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsMoving", false);
            animator.SetFloat("VerticalSpeed", 0f);
            animator.SetFloat("HorizontalSpeed", 0f);
            return;
        }

        // Lógica normal de movimiento
        if (moveInput.x > 0) sr.flipX = false;
        if (moveInput.x < 0) sr.flipX = true;

        bool isPressingKeys = moveInput.magnitude > 0.1f;
        float absHorizontal = Mathf.Abs(moveInput.x);

        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        animator.SetBool("IsMoving", isPressingKeys);
        animator.SetFloat("VerticalSpeed", moveInput.y);
        animator.SetFloat("HorizontalSpeed", absHorizontal);

        Debug.Log("VerticalSpeed: " + moveInput.y + " HorizontalSpeed: " + Mathf.Abs(moveInput.x));
    }

    void FixedUpdate()
    {
        // Si el tiempo es 0 o no hay control, las físicas no se aplican
        if (!canControl || Time.timeScale == 0) return;

        rb.AddForce(moveInput * moveSpeed, ForceMode2D.Force);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // Método público para quitar/devolver el control desde el GameUI
    public void SetControl(bool value)
    {
        canControl = value;

        if (!value)
        {
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero; // Frenamos en seco la inercia física
        }
    }

    public void TriggerDeath()
    {
        animator.SetTrigger("Die");
    }
}