using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("Exit Settings")]
    public bool requiresAllCandles = false;
    public bool requiresKey = true;
    public string requiredKeyName = "BasementKey";

    [Header("Runtime References")]
    // The solid (non-trigger) collider that blocks the player when the door is closed.
    public Collider blockingCollider;

    private bool playerInZone = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            CheckExitConditions();

            // If the player already meets the exit conditions, immediately win
            if (CanEscape())
            {
                GameManager.instance.TriggerWin();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            UIManager.instance.HideInteractionPrompt();
        }
    }

    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            TryEscape();
        }
    }

    void CheckExitConditions()
    {
        if (CanEscape())
        {
            UIManager.instance.ShowInteractionPrompt("Press E to ESCAPE!");
            return;
        }

        // Build a friendly locked message
        if (requiresAllCandles && GameManager.instance.candlesCollected < GameManager.instance.candlesNeeded)
        {
            int remaining = GameManager.instance.candlesNeeded - GameManager.instance.candlesCollected;
            UIManager.instance.ShowInteractionPrompt("Door won't open — find " + remaining + " more candle(s)");
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
                return;
            }
        }

        // Generic fallback
        UIManager.instance.ShowInteractionPrompt("Door is locked.");
    }

    void TryEscape()
    {
        if (CanEscape())
        {
            Debug.Log("PLAYER ESCAPED!");
            if (blockingCollider != null)
                blockingCollider.enabled = false;
            GameManager.instance.TriggerWin();
        }
        else
        {
            Debug.Log("Cannot escape yet!");
        }
    }

    bool CanEscape()
    {
        // Check candles
        if (requiresAllCandles && GameManager.instance.candlesCollected < GameManager.instance.candlesNeeded)
            return false;

        // Check key
        if (requiresKey)
        {
            bool hasKey = requiredKeyName == "BasementKey" 
                ? GameManager.instance.hasBasementKey 
                : GameManager.instance.hasSmallKey;
            
            if (!hasKey)
                return false;
        }

        return true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
    }
}