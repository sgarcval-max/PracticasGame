using UnityEngine;
using UnityEngine.InputSystem;

public class DiverHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    public float invincibleTime = 1.5f;
    private float invincibleTimer = 0f;
    private bool isInvincible = false;

    private SpriteRenderer sr;
    private GameUI gameUI;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
        gameUI = FindFirstObjectByType<GameUI>();
    }

    void Start()
    {
        gameUI?.UpdateHealth(currentHealth, maxHealth);
    }

    void Update()
    {
        // Tecla B para volver a la base (fuera del if de invencibilidad)
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            Die();
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            sr.enabled = Mathf.Sin(invincibleTimer * 20f) > 0;

            if (invincibleTimer <= 0f)
            {
                isInvincible = false;
                sr.enabled = true;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        isInvincible = true;
        invincibleTimer = invincibleTime;

        gameUI?.UpdateHealth(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Activar animación de muerte
        DiverController controller = GetComponent<DiverController>();
        if (controller != null)
            controller.TriggerDeath();

        DiverInventory inventory = GetComponent<DiverInventory>();
        WaveManager waveManager = FindFirstObjectByType<WaveManager>();

        if (GameManager.Instance != null)
            GameManager.Instance.SavePlayerData(inventory, waveManager);

        // Esperamos a que termine la animación antes de mostrar Game Over
        StartCoroutine(DeathCoroutine());
    }

    System.Collections.IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);

        GameUI gameUI = FindFirstObjectByType<GameUI>();
        if (gameUI != null)
            gameUI.ShowGameOver();

        gameObject.SetActive(false);
    }

    // Permite activar o desactivar la invencibilidad desde fuera
    public void SetInvincible(bool value)
    {
        isInvincible = value;
        if (!value)
        {
            sr.enabled = true;
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        gameUI?.UpdateHealth(currentHealth, maxHealth);
        Debug.Log("Vida actual: " + currentHealth + "/" + maxHealth);
    }

}