using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public float maxBattery = 100f;
    public float drainRate = 2f;
    public KeyCode toggleKey = KeyCode.F;

    [Header("References")]
    public Light flashlight;

    [Header("Mobile UI")]
    public Button flashlightToggleButton;   // Assign in Inspector (your on-screen button)
    public Image flashlightButtonIcon;      // Optional: icon on the button to swap on/off

    [Header("Button Icons (optional)")]
    public Sprite iconOn;   // flashlight ON sprite
    public Sprite iconOff;  // flashlight OFF sprite

    private float currentBattery;
    private bool isOn = true;

    void Start()
    {
        currentBattery = maxBattery;
        flashlight.enabled = true;

        // Hook up the on-screen button
        if (flashlightToggleButton != null)
        {
            flashlightToggleButton.onClick.AddListener(ToggleFlashlight);
        }

        UpdateButtonIcon();

        if (UIManager.instance != null)
            UIManager.instance.UpdateBattery(currentBattery, maxBattery);
    }

    void Update()
    {
        // Keyboard toggle still works (F key)
        if (Input.GetKeyDown(toggleKey))
            ToggleFlashlight();

        HandleBatteryDrain();

        if (UIManager.instance != null)
            UIManager.instance.UpdateBattery(currentBattery, maxBattery);
    }

    // ─── Public: called by button OR keyboard ────────────────────────────────
    public void ToggleFlashlight()
    {
        // Can't turn on if dead battery
        if (!isOn && currentBattery <= 0) return;

        isOn = !isOn;
        flashlight.enabled = isOn;

        UpdateButtonIcon();

        if (AudioManager.instance != null)
            AudioManager.instance.PlayFlashlightClick();
    }

    // ─── Battery drain ────────────────────────────────────────────────────────
    void HandleBatteryDrain()
    {
        if (isOn && currentBattery > 0)
        {
            currentBattery -= drainRate * Time.deltaTime;

            float batteryPercent = currentBattery / maxBattery;
            flashlight.intensity = batteryPercent * 3f;

            if (currentBattery <= 0)
            {
                currentBattery = 0;
                isOn = false;
                flashlight.enabled = false;
                UpdateButtonIcon();
            }
        }
    }

    // ─── Called by PickupItem when player grabs a battery ────────────────────
    public void AddBattery(float amount)
    {
        currentBattery = Mathf.Min(currentBattery + amount, maxBattery);

        // Auto turn on if it was off due to dead battery
        if (currentBattery > 0 && !isOn)
        {
            isOn = true;
            flashlight.enabled = true;
            UpdateButtonIcon();
        }
    }

    public float GetBatteryPercent()
    {
        return currentBattery / maxBattery;
    }

    // ─── Swap button icon ─────────────────────────────────────────────────────
    void UpdateButtonIcon()
    {
        if (flashlightButtonIcon == null) return;

        if (isOn && iconOn != null)
            flashlightButtonIcon.sprite = iconOn;
        else if (!isOn && iconOff != null)
            flashlightButtonIcon.sprite = iconOff;
    }
}