using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("Exit Settings")]
    public bool requiresAllCandles = false;
    public bool requiresKey = true;
    public string requiredKeyName = "BasementKey";
    public string nextSceneName = ""; // Set to "Level2" in Level 1. Leave blank in Level 2 to show win screen.

    [Header("Runtime References")]
    // The solid (non-trigger) collider that blocks the player when the door is closed.
    public Collider blockingCollider;

    private bool playerInZone = false;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("🚪 Something entered exit door trigger: " + other.name + " (Tag: " + other.tag + ")");
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("=== PLAYER ENTERED EXIT DOOR ===");
            Debug.Log("Requires All Candles: " + requiresAllCandles);
            Debug.Log("Requires Key: " + requiresKey);
            Debug.Log("Required Key Name: " + requiredKeyName);
            Debug.Log("Has Basement Key: " + GameManager.instance.hasBasementKey);
            Debug.Log("Has Small Key: " + GameManager.instance.hasSmallKey);
            Debug.Log("Candles: " + GameManager.instance.candlesCollected + "/" + GameManager.instance.candlesNeeded);
            Debug.Log("Can Escape: " + CanEscape());
            
            playerInZone = true;
            CheckExitConditions();

            // If the player already meets the exit conditions, immediately escape
            if (CanEscape())
            {
                Debug.Log("🎉 Player can escape! Triggering escape immediately!");
                TryEscape();
            }
            else
            {
                Debug.Log("❌ Cannot escape - requirements not met");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left exit door zone");
            playerInZone = false;
            UIManager.instance.HideInteractionPrompt();
        }
    }

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E at exit door");
            TryEscape();
        }
    }

    void CheckExitConditions()
    {
        if (CanEscape())
        {
            UIManager.instance.ShowInteractionPrompt("Press E to ESCAPE!");
            Debug.Log("✅ Showing ESCAPE prompt");
            return;
        }

        // Build a friendly locked message
        if (requiresAllCandles && GameManager.instance.candlesCollected < GameManager.instance.candlesNeeded)
        {
            int remaining = GameManager.instance.candlesNeeded - GameManager.instance.candlesCollected;
            UIManager.instance.ShowInteractionPrompt("Door won't open — find " + remaining + " more candle(s)");
            Debug.Log("❌ Need more candles");
            return;
        }

        if (requiresKey)
        {
            bool hasKey = requiredKeyName == "BasementKey" 
                ? GameManager.instance.hasBasementKey 
                : GameManager.instance.hasSmallKey;

            if (!hasKey)
            {
                UIManager.instance.ShowInteractionPrompt("Door won't open — find the key to escape.");
                Debug.Log("❌ Need the key: " + requiredKeyName);
                return;
            }
        }

        // Generic fallback
        UIManager.instance.ShowInteractionPrompt("Door is locked.");
        Debug.Log("❌ Door is locked (generic)");
    }

    void TryEscape()
    {
        if (CanEscape())
        {
            Debug.Log("🎉🎉🎉 PLAYER ESCAPED! 🎉🎉🎉");
            if (blockingCollider != null)
                blockingCollider.enabled = false;

            // If a next scene is configured, load it. Otherwise show the win screen.
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                Debug.Log("Loading next scene: " + nextSceneName);
                Time.timeScale = 1f; // Unfreeze time before loading
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                GameManager.instance.TriggerWin();
            }
        }
        else
        {
            Debug.Log("❌ Cannot escape yet! Missing requirements.");
        }
    }

    bool CanEscape()
    {
        // Check candles
        if (requiresAllCandles && GameManager.instance.candlesCollected < GameManager.instance.candlesNeeded)
        {
            Debug.Log("CanEscape: FALSE - Need more candles");
            return false;
        }

        // Check key
        if (requiresKey)
        {
            bool hasKey = requiredKeyName == "BasementKey" 
                ? GameManager.instance.hasBasementKey 
                : GameManager.instance.hasSmallKey;
            
            if (!hasKey)
            {
                Debug.Log("CanEscape: FALSE - Need key: " + requiredKeyName);
                return false;
            }
        }

        Debug.Log("CanEscape: TRUE - All requirements met!");
        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}