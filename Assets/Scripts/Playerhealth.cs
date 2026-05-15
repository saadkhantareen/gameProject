using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Damage Effects")]
    public float damageScreenFlashDuration = 0.3f;
    public Color damageColor = new Color(1f, 0f, 0f, 0.3f);

    // We'll add UI later
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UIManager.instance.UpdateHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage! Health: " + currentHealth);

        // ADD THIS LINE:
        UIManager.instance.UpdateHealth(currentHealth, maxHealth);

        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        isDead = true;
        Debug.Log("YOU DIED!");
        AudioManager.instance.PlayDeath();
        
        // Disable player movement
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        
        // Disable any movement script (try common script names)
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script.GetType().Name.Contains("Movement") || 
                script.GetType().Name.Contains("Controller") ||
                script.GetType().Name.Contains("Player"))
            {
                script.enabled = false;
            }
        }

        // Trigger game over
        GameManager.instance.TriggerGameOver();
    }

    System.Collections.IEnumerator DamageFlash()
    {
        // We'll add a UI overlay later, for now just log it
        yield return new WaitForSeconds(damageScreenFlashDuration);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Healed! Health: " + currentHealth);
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
}