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

    private float currentBattery;
    private bool isOn = true;

    void Start()
    {
        currentBattery = maxBattery;
        flashlight.enabled = true;
        
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateBattery(currentBattery, maxBattery);
        }
    }

    void Update()
    {
        HandleToggle();
        HandleBatteryDrain();
        
        // Update UI
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateBattery(currentBattery, maxBattery);
        }
    }

    void HandleToggle()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
            
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayFlashlightClick();
            }
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