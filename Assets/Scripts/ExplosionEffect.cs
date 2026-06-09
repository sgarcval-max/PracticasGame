using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    private Animator animator;
    private float clipLength;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Obtenemos la duración de la animación
        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips.Length > 0)
            clipLength = clips[0].clip.length;
        else
            clipLength = 1f;

        // Nos destruimos cuando acaba la animación
        Destroy(gameObject, clipLength);
    }
}
