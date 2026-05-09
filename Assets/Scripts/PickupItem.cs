using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType
    {
        Battery,
        Candle,
        Key,
        JournalPage
    }

    [Header("Item Settings")]
    public ItemType itemType;
    public float batteryAmount = 30f;  // Only used if this is a Battery
    public string itemName = "Item";

    // This runs when the player picks it up
    public void OnPickup()
    {
        // Find the GameManager and tell it what was picked up
        GameManager.instance.CollectItem(itemType, itemName);

        // If it is a battery, refill the flashlight
        if (itemType == ItemType.Battery)
        {
            FlashlightController flashlight = FindObjectOfType<FlashlightController>();
            if (flashlight != null)
            {
                flashlight.AddBattery(batteryAmount);
            }
        }

        // Destroy the item from the world
        Destroy(gameObject);
    }
}