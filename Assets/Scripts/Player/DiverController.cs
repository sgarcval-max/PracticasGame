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

    private bool canControl = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.linearDamping = linearDrag;

        // Por defecto empieza en Normal, para que SÍ se pause con el Menú de Pausa
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.Normal;
        }
    }

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
        if (!canControl)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsMoving", false);
            animator.SetFloat("VerticalSpeed", 0f);
            animator.SetFloat("HorizontalSpeed", 0f);
            return;
        }

        if (moveInput.x > 0) sr.flipX = false;
        if (moveInput.x < 0) sr.flipX = true;

        bool isPressingKeys = moveInput.magnitude > 0.1f;
        float absHorizontal = Mathf.Abs(moveInput.x);

        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
        animator.SetBool("IsMoving", isPressingKeys);
        animator.SetFloat("VerticalSpeed", moveInput.y);
        animator.SetFloat("HorizontalSpeed", absHorizontal);
    }

    void FixedUpdate()
    {
        if (!canControl || Time.timeScale == 0) return;

        rb.AddForce(moveInput * moveSpeed, ForceMode2D.Force);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    public void SetControl(bool value)
    {
        canControl = value;
        if (!value)
        {
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }

    // --- NUEVO MÉTODO EXCLUSIVO PARA LA VICTORIA ---
    public void SetAnimatorIgnoreTime(bool ignore)
    {
        if (animator != null)
        {
            animator.updateMode = ignore ? AnimatorUpdateMode.UnscaledTime : AnimatorUpdateMode.Normal;
        }
    }

    public void TriggerDeath()
    {
        // Activamos el trigger del Animator
        animator.SetTrigger("Die");

        // Nos aseguramos de que no haya inercia
        rb.linearVelocity = Vector2.zero;
        canControl = false;
    }
}