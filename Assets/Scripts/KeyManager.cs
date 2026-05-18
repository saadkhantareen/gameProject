using UnityEngine;
using TMPro;

public class KeyManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI keysCounterText;
    public TextMeshProUGUI interactionPromptText;
    public TextMeshProUGUI notificationText;

    private int keysCollected = 0;
    public int totalKeys = 1; // Set to 2 in Level 2 via the Inspector

    void Start()
    {
        UpdateKeyUI();
        interactionPromptText.gameObject.SetActive(false);
        notificationText.gameObject.SetActive(false);
    }

    public void PickUpKey()
    {
        keysCollected++;
        UpdateKeyUI();
        ShowNotification("Picked up the key!\nNow go to the Door outside the house to escape.");
    }

    private void UpdateKeyUI()
    {
        keysCounterText.text = $"keys: {keysCollected}/{totalKeys}";
    }

    public void ShowInteractionPrompt()
    {
        interactionPromptText.gameObject.SetActive(true);
    }

    public void HideInteractionPrompt()
    {
        interactionPromptText.gameObject.SetActive(false);
    }

    public void ShowNotification(string message)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        Invoke(nameof(HideNotification), 5f);
    }

    private void HideNotification()
    {
        notificationText.gameObject.SetActive(false);
    }
}
