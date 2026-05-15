using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public float maxBattery = 100f;
    public float drainRate = 2f;      // Battery lost per second when ON
    public KeyCode toggleKey = KeyCode.F;

    [Header("References")]
    public Light flashlight;           // Drag your Flashlight here

    // We will connect UI later — leave this for now
    // public Slider batterySlider;

    private float currentBattery;
    private bool isOn = true;

    void Start()
    {
        currentBattery = maxBattery;
        flashlight.enabled = true;
        UIManager.instance.UpdateBattery(currentBattery, maxBattery);
    }

    void Update()
    {
        HandleToggle();
        HandleBatteryDrain();
        // After battery changes, add:
        UIManager.instance.UpdateBattery(currentBattery, maxBattery);
    }

    void HandleToggle()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
            AudioManager.instance.PlayFlashlightClick();
        }
    }

    void HandleBatteryDrain()
    {
        if (isOn && currentBattery > 0)
        {
            currentBattery -= drainRate * Time.deltaTime;

            // Dim the light as battery dies
            float batteryPercent = currentBattery / maxBattery;
            flashlight.intensity = batteryPercent * 3f;

            if (currentBattery <= 0)
            {
                currentBattery = 0;
                isOn = false;
                flashlight.enabled = false;
            }
        }
    }

    // Call this from other scripts when player picks up a battery
    public void AddBattery(float amount)
    {
        currentBattery = Mathf.Min(currentBattery + amount, maxBattery);
        if (currentBattery > 0 && !isOn)
        {
            isOn = true;
            flashlight.enabled = true;
        }
    }

    public float GetBatteryPercent()
    {
        return currentBattery / maxBattery;
    }
}