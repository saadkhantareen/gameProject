using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // This lets other scripts find the GameManager easily
    public static GameManager instance;

    [Header("Game Progress")]
    public int candlesCollected = 0;
    public int candlesNeeded = 4;
    public bool hasBasementKey = false;
    public bool hasSmallKey = false;
    public int journalPagesFound = 0;

    [Header("Game State")]
    public bool gameOver = false;
    public bool gameWon = false;

    void Awake()
    {
        // Make sure only one GameManager exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CollectItem(PickupItem.ItemType type, string itemName)
    {
        Debug.Log("Picked up: " + itemName); // We will replace this with real UI later

        switch (type)
        {
            case PickupItem.ItemType.Battery:
                Debug.Log("Battery collected!");
                break;

            case PickupItem.ItemType.Candle:
                candlesCollected++;
                Debug.Log("Candles: " + candlesCollected + "/" + candlesNeeded);
                break;

            case PickupItem.ItemType.Key:
                if (itemName == "BasementKey")
                    hasBasementKey = true;
                else if (itemName == "SmallKey")
                    hasSmallKey = true;
                Debug.Log(itemName + " obtained!");
                break;

            case PickupItem.ItemType.JournalPage:
                journalPagesFound++;
                Debug.Log("Journal pages: " + journalPagesFound);
                break;
        }
    }

    public void TriggerGameOver()
    {
        if (gameOver) return;
        gameOver = true;
        Debug.Log("GAME OVER");
        // We will add proper game over screen in Phase 8
        Invoke("RestartGame", 2f);
    }

    public void TriggerWin()
    {
        if (gameWon) return;
        gameWon = true;
        Debug.Log("YOU ESCAPED!");
        // We will add win screen in Phase 8
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}