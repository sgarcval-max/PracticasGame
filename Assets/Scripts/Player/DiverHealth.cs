using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DiverHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    public float invincibleTime = 1.5f;
    private float invincibleTimer = 0f;
    private bool isInvincible = false;

    private SpriteRenderer sr;
    private GameUI gameUI;

    private bool isShieldActive = false;

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
        if (Keyboard.current.bKey.wasPressedThisFrame && !TutorialSceneFlag.IsTutorial)
        {
            // Paramos el countdown si está activo
            if (AudioManager.Instance != null && AudioManager.Instance.countdownSource != null)
            {
                AudioManager.Instance.countdownSource.Stop();
                AudioManager.Instance.countdownSource.loop = false;
            }
            GameUI.isCountdownActive = false;

            DiverInventory inventory = GetComponent<DiverInventory>();
            WaveManager waveManager = FindFirstObjectByType<WaveManager>();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.currentWave = waveManager != null ? waveManager.GetCurrentWave() : 0;
                GameManager.Instance.SavePlayerData(inventory, waveManager);
            }

            Time.timeScale = 1f;

            if (SceneTransition.Instance != null)
                SceneTransition.Instance.TransitionToBase();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("BaseScene");
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
        // No recibe daño si el escudo está activo
        if (isInvincible || isShieldActive) return;

        currentHealth -= damage;
        isInvincible = true;
        invincibleTimer = invincibleTime;

        gameUI?.UpdateHealth(currentHealth, maxHealth);

        // Tutorial
        TutorialDamage td = GetComponent<TutorialDamage>();
        if (td != null) td.OnDamageReceived();

        // Mostrar viñeta roja
        if (DamageVignette.Instance != null)
            DamageVignette.Instance.ShowDamage();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        DiverController controller = GetComponent<DiverController>();
        if (controller != null)
            controller.TriggerDeath();

        if (!TutorialSceneFlag.IsTutorial)
        {
            // Solo guardamos progreso si NO estamos en el tutorial
            if (GameManager.Instance != null)
            {
                GameManager.Instance.caughtFish = new List<FishType>(GameManager.Instance.caughtFishBackup);
                GameManager.Instance.collectedTreasure = GameManager.Instance.collectedTreasureBackup;

                DiverInventory inventory = GetComponent<DiverInventory>();
                WaveManager waveManager = FindFirstObjectByType<WaveManager>();
                GameManager.Instance.SavePlayerData(inventory, waveManager, true);
            }
        }

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

    public void SetInvincible(bool value)
    {
        isShieldActive = value;
        if (!value)
            sr.enabled = true;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        gameUI?.UpdateHealth(currentHealth, maxHealth);
        Debug.Log("Vida actual: " + currentHealth + "/" + maxHealth);
    }
}