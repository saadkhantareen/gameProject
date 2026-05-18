# Full Source Code — Part 1 (Core Managers)

## GameManager.cs
```csharp
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
    { aiSpeedMultiplier = 0.7f, aiHearingMultiplier = 0.8f, aiVisionMultiplier = 0.8f, flashlightDrainMultiplier = 0.5f };
    public DifficultySettings normalSettings = new DifficultySettings 
    { aiSpeedMultiplier = 1f, aiHearingMultiplier = 1f, aiVisionMultiplier = 1f, flashlightDrainMultiplier = 1f };
    public DifficultySettings hardSettings = new DifficultySettings 
    { aiSpeedMultiplier = 1.3f, aiHearingMultiplier = 1.5f, aiVisionMultiplier = 1.3f, flashlightDrainMultiplier = 1.5f };
    public DifficultySettings extremeSettings = new DifficultySettings 
    { aiSpeedMultiplier = 1.5f, aiHearingMultiplier = 2f, aiVisionMultiplier = 1.5f, flashlightDrainMultiplier = 2f };

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ApplyDifficultySettings();
        if (UIManager.instance != null)
            UIManager.instance.UpdateCandles(candlesCollected, candlesNeeded);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) TogglePause();
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
                if (UIManager.instance != null)
                    UIManager.instance.UpdateCandles(candlesCollected, candlesNeeded);
                if (candlesCollected >= candlesNeeded)
                    Debug.Log("All candles collected! Find the exit!");
                break;
            case PickupItem.ItemType.Key:
                if (itemName == "BasementKey") hasBasementKey = true;
                else if (itemName == "SmallKey") hasSmallKey = true;
                break;
            case PickupItem.ItemType.JournalPage:
                journalPagesFound++;
                break;
        }
    }

    public void TriggerGameOver()
    {
        if (gameOver) return;
        gameOver = true;
        Time.timeScale = 0f;
        if (ScreenManager.instance != null) ScreenManager.instance.ShowGameOver();
    }
    
    public void TriggerWin()
    {
        if (gameWon) return;
        gameWon = true;
        Time.timeScale = 0f;
        if (ScreenManager.instance != null) ScreenManager.instance.ShowWinScreen();
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void SetDifficulty(Difficulty newDifficulty)
    {
        currentDifficulty = newDifficulty;
        ApplyDifficultySettings();
    }

    void ApplyDifficultySettings()
    {
        DifficultySettings settings = GetCurrentDifficultySettings();
        GrannyStyleAI[] enemies = Object.FindObjectsByType<GrannyStyleAI>(FindObjectsSortMode.None);
        foreach (GrannyStyleAI enemy in enemies)
        {
            enemy.chaseSpeed *= settings.aiSpeedMultiplier;
            enemy.patrolSpeed *= settings.aiSpeedMultiplier;
            enemy.hearingRange *= settings.aiHearingMultiplier;
            enemy.chaseRange *= settings.aiVisionMultiplier;
        }
        FlashlightController flashlight = Object.FindFirstObjectByType<FlashlightController>();
        if (flashlight != null) flashlight.drainRate *= settings.flashlightDrainMultiplier;
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
```

## AudioManager.cs
```csharp
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip backgroundMusic;

    [Header("SFX Clips")]
    public AudioClip footstepSound;
    public AudioClip doorSound;
    public AudioClip pickupSound;
    public AudioClip flashlightClick;
    public AudioClip deathScream;
    public AudioClip grannyFootstep;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start() { PlayMusic(backgroundMusic); }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        { musicSource.clip = clip; musicSource.loop = true; musicSource.Play(); }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null) sfxSource.PlayOneShot(clip);
    }

    public void PlayFootstep() { PlaySFX(footstepSound); }
    public void PlayDoor() { PlaySFX(doorSound); }
    public void PlayPickup() { PlaySFX(pickupSound); }
    public void PlayFlashlightClick() { PlaySFX(flashlightClick); }
    public void PlayDeath() { PlaySFX(deathScream); }
    public void PlayGrannyFootstep() { PlaySFX(grannyFootstep); }
}
```

## UIManager.cs
```csharp
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("HUD Elements")]
    public Slider healthSlider;
    public Slider batterySlider;
    public TextMeshProUGUI candleText;
    public Image crosshair;
    public TextMeshProUGUI interactionText;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (interactionText != null) interactionText.gameObject.SetActive(false);
    }

    public void UpdateHealth(float current, float max)
    { if (healthSlider != null) healthSlider.value = current / max; }

    public void UpdateBattery(float current, float max)
    { if (batterySlider != null) batterySlider.value = current / max; }

    public void UpdateCandles(int current, int total)
    { if (candleText != null) candleText.text = "Candles: " + current + "/" + total; }

    public void ShowInteractionPrompt(string message)
    {
        if (interactionText != null)
        { interactionText.text = message; interactionText.gameObject.SetActive(true); }
    }

    public void HideInteractionPrompt()
    { if (interactionText != null) interactionText.gameObject.SetActive(false); }
}
```

## ScreenManager.cs
```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;

    [Header("Screen References")]
    public GameObject gameOverScreen;
    public GameObject winScreen;
    public GameObject pauseMenu;
    private bool isPaused = false;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (winScreen != null) winScreen.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverScreen != null)
        { gameOverScreen.SetActive(true); Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
    }

    public void ShowWinScreen()
    {
        if (winScreen != null)
        { winScreen.SetActive(true); Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
    }

    public void RestartGame()
    { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }

    public void LoadMainMenu()
    { Time.timeScale = 1f; SceneManager.LoadScene("MainMenu"); }

    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenu != null) pauseMenu.SetActive(true);
        Time.timeScale = 0f; Cursor.lockState = CursorLockMode.None; Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenu != null) pauseMenu.SetActive(false);
        Time.timeScale = 1f; Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        { if (isPaused) ResumeGame(); else PauseGame(); }
    }
}
```

## MainMenuController.cs
```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }

    public void StartGame() { SceneManager.LoadScene("SampleScene"); }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
```
