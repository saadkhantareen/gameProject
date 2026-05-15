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
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Hide interaction text at start
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    // Called by PlayerHealth
    public void UpdateHealth(float current, float max)
    {
        if (healthSlider != null)
            healthSlider.value = current / max;
    }

    // Called by FlashlightController
    public void UpdateBattery(float current, float max)
    {
        if (batterySlider != null)
            batterySlider.value = current / max;
    }

    // Called by GameManager
    public void UpdateCandles(int current, int total)
    {
        if (candleText != null)
            candleText.text = "Candles: " + current + "/" + total;
    }

    // Called by PlayerInteraction
    public void ShowInteractionPrompt(string message)
    {
        if (interactionText != null)
        {
            interactionText.text = message;
            interactionText.gameObject.SetActive(true);
        }
    }

    public void HideInteractionPrompt()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }
}