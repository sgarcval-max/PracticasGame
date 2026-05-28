using UnityEngine;

public class FishAnimator : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb == null || animator == null) return;

        // Voltear sprite según dirección
        if (rb.linearVelocity.x > 0.1f)
            sr.flipX = false;
        else if (rb.linearVelocity.x < -0.1f)
            sr.flipX = true;
    }

    public void TriggerAttack()
    {
        if (animator != null)
            animator.SetBool("IsAttacking", true);
    }

    public void StopAttack()
    {
        if (animator != null)
            animator.SetBool("IsAttacking", false);
    }
}
