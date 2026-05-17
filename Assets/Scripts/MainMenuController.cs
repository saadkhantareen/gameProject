using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        // Unlock cursor for menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Make sure time is running
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene("saad"); // Change if your game scene has different name
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        // This works in editor too:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}   