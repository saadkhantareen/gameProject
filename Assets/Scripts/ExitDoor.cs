using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("Exit Settings")]
    public bool requiresAllCandles = true;
    public bool requiresKey = false;
    public string requiredKeyName = "BasementKey";

    private bool playerInZone = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            CheckExitConditions();
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
        }
        else
        {
            string message = "EXIT LOCKED - ";
            
            if (requiresAllCandles && GameManager.instance.candlesCollected < GameManager.instance.candlesNeeded)
            {
                int remaining = GameManager.instance.candlesNeeded - GameManager.instance.candlesCollected;
                message += "Find " + remaining + " more candle(s)";
            }
            else if (requiresKey)
            {
                bool hasKey = requiredKeyName == "BasementKey" 
                    ? GameManager.instance.hasBasementKey 
                    : GameManager.instance.hasSmallKey;
                
                if (!hasKey)
                    message += "Need " + requiredKeyName;
            }
            
            UIManager.instance.ShowInteractionPrompt(message);
        }
    }

    void TryEscape()
    {
        if (CanEscape())
        {
            Debug.Log("PLAYER ESCAPED!");
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