using UnityEngine;

public class PlayerMovementSFX : MonoBehaviour
{
    public AudioClip swimSound;
    public float minSpeed = 0.5f;

    private Rigidbody2D rb;
    private bool isPlaying = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rb == null || AudioManager.Instance == null) return;

        bool shouldPlay = rb.linearVelocity.magnitude > minSpeed;

        if (shouldPlay && !isPlaying)
        {
            isPlaying = true;
            AudioManager.Instance.musicSource.loop = true;
            // Reproducimos el sonido de nadar en loop
            if (swimSound != null)
                AudioManager.Instance.sfxSource.clip = swimSound;
            AudioManager.Instance.sfxSource.loop = true;
            AudioManager.Instance.sfxSource.Play();
        }
        else if (!shouldPlay && isPlaying)
        {
            isPlaying = false;
            AudioManager.Instance.sfxSource.Stop();
        }
    }
}