using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
    public bool isPaused = false;

    [Header("Difficulty Settings")]
    public Difficulty currentDifficulty = Difficulty.Normal;
    
    public enum Difficulty { Easy, Normal, Hard, Extreme }

    [System.Serializable]
    public class DifficultySettings
    {
        public float aiSpeedMultiplier = 1f;
        public float aiHearingMultiplier = 1f;
        public float aiVisionMultiplier = 1f;
        public float flashlightDrainMultiplier = 1f;
    }

    public DifficultySettings easySettings = new DifficultySettings 
    { 
        aiSpeedMultiplier = 0.7f, 
        aiHearingMultiplier = 0.8f,
        aiVisionMultiplier = 0.8f,
        flashlightDrainMultiplier = 0.5f
    };

    public DifficultySettings normalSettings = new DifficultySettings 
    { 
        aiSpeedMultiplier = 1f, 
        aiHearingMultiplier = 1f,
        aiVisionMultiplier = 1f,
        flashlightDrainMultiplier = 1f
    };

    public DifficultySettings hardSettings = new DifficultySettings 
    { 
        aiSpeedMultiplier = 1.3f, 
        aiHearingMultiplier = 1.5f,
        aiVisionMultiplier = 1.3f,
        flashlightDrainMultiplier = 1.5f
    };

    public DifficultySettings extremeSettings = new DifficultySettings 
    { 
        aiSpeedMultiplier = 1.5f, 
        aiHearingMultiplier = 2f,
        aiVisionMultiplier = 1.5f,
        flashlightDrainMultiplier = 2f
    };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ApplyDifficultySettings();
        
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateCandles(candlesCollected, candlesNeeded);
        }
    }

    void Update()
    {
        // Pause game with ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void CollectItem(PickupItem.ItemType type, string itemName)
    {
        Debug.Log("Picked up: " + itemName);

        switch (type)
        {
            case PickupItem.ItemType.Battery:
                Debug.Log("Battery collected!");
                break;

            case PickupItem.ItemType.Candle:
                candlesCollected++;
                Debug.Log("Candles: " + candlesCollected + "/" + candlesNeeded);
                
                if (UIManager.instance != null)
                {
                    UIManager.instance.UpdateCandles(candlesCollected, candlesNeeded);
                }
                
                if (candlesCollected >= candlesNeeded)
                {
                    Debug.Log("All candles collected! Find the exit!");
                }
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
        Debug.Log("=== GAME OVER ===");
        
        Time.timeScale = 0f;
        
        if (ScreenManager.instance != null)
        {
            ScreenManager.instance.ShowGameOver();
        }
    }
    
    public void TriggerWin()
    {
        if (gameWon) return;
        gameWon = true;
        Debug.Log("=== YOU ESCAPED! ===");
        
        Time.timeScale = 0f;
        
        if (ScreenManager.instance != null)
        {
            ScreenManager.instance.ShowWinScreen();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        Debug.Log(isPaused ? "PAUSED" : "RESUMED");
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        currentDifficulty = newDifficulty;
        ApplyDifficultySettings();
        Debug.Log("Difficulty set to: " + newDifficulty);
    }

    void ApplyDifficultySettings()
    {
        DifficultySettings settings = GetCurrentDifficultySettings();

        // Apply to all AI enemies
        GrannyStyleAI[] enemies = Object.FindObjectsByType<GrannyStyleAI>(FindObjectsSortMode.None);
        foreach (GrannyStyleAI enemy in enemies)
        {
            enemy.chaseSpeed *= settings.aiSpeedMultiplier;
            enemy.patrolSpeed *= settings.aiSpeedMultiplier;
            enemy.hearingRange *= settings.aiHearingMultiplier;
            enemy.chaseRange *= settings.aiVisionMultiplier;
        }

        // Apply to flashlight
        FlashlightController flashlight = Object.FindFirstObjectByType<FlashlightController>();
        if (flashlight != null)
        {
            flashlight.drainRate *= settings.flashlightDrainMultiplier;
        }
    }

    DifficultySettings GetCurrentDifficultySettings()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy: return easySettings;
            case Difficulty.Normal: return normalSettings;
            case Difficulty.Hard: return hardSettings;
            case Difficulty.Extreme: return extremeSettings;
            default: return normalSettings;
        }
    }
}