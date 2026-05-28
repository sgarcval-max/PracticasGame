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

        // No tocamos el flip aquí, lo maneja FishEnemy
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
