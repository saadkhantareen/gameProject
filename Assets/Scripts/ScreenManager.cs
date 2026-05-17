using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;

    [Header("Screen References")]
    public GameObject gameOverScreen;
    public GameObject winScreen;
    public GameObject pauseMenu; // ADD THIS
    private bool isPaused = false; // ADD THIS

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Make sure screens are hidden at start
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);
        if (winScreen != null)
            winScreen.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ShowWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Called by Restart button
    public void RestartGame()
    {
        Time.timeScale = 1f; // Unpause
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Called by Main Menu button
    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Unpause
        SceneManager.LoadScene("MainMenu"); // We'll create this scene later
    }
    public void PauseGame()
    {
        isPaused = true;
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
        
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update() // ADD THIS WHOLE METHOD
    {
        // Toggle pause with P
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
}