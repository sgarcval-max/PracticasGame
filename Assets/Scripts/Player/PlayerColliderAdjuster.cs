using UnityEngine;

public class PlayerColliderAdjuster : MonoBehaviour
{
    private CapsuleCollider2D col;
    private Animator animator;

    [Header("Tamaños del collider por estado")]
    public Vector2 idleSize = new Vector2(0.5f, 0.8f);
    public Vector2 idleOffset = new Vector2(0f, 0f);

    public Vector2 movingSize = new Vector2(0.6f, 0.7f);
    public Vector2 movingOffset = new Vector2(0f, -0.05f);

    public Vector2 verticalSize = new Vector2(0.5f, 0.9f);
    public Vector2 verticalOffset = new Vector2(0f, 0f);

    void Awake()
    {
        col = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (col == null || animator == null) return;

        float horizontal = animator.GetFloat("HorizontalSpeed");
        float vertical = animator.GetFloat("VerticalSpeed");
        bool isMoving = animator.GetBool("IsMoving");

        if (!isMoving)
        {
            col.size = idleSize;
            col.offset = idleOffset;
        }
        else if (Mathf.Abs(vertical) > 0.5f && horizontal < 0.1f)
        {
            // Solo arriba o solo abajo
            col.size = verticalSize;
            col.offset = verticalOffset;
        }
        else
        {
            // Movimiento horizontal
            col.size = movingSize;
            col.offset = movingOffset;
        }
    }
}
