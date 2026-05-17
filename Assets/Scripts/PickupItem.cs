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

    [Header("Sound Settings")]
    public bool makesNoiseWhenDropped = true;
    public float dropNoiseRadius = 15f;

    private SoundEmitter soundEmitter;
    private Rigidbody rb;

    void Start()
    {
        // Add sound emitter if this item makes noise
        if (makesNoiseWhenDropped)
        {
            soundEmitter = gameObject.AddComponent<SoundEmitter>();
            soundEmitter.soundRadius = dropNoiseRadius;
        }

        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Make noise when item hits the ground (only if dropped, not initial placement)
        if (makesNoiseWhenDropped && rb != null && !rb.isKinematic)
        {
            // Only make noise if impact was strong enough
            if (collision.relativeVelocity.magnitude > 2f)
            {
                if (soundEmitter != null)
                {
                    soundEmitter.EmitSound(transform.position, dropNoiseRadius);
                    Debug.Log(itemName + " was dropped! Granny might have heard that...");
                }
            }
        }
    }

    // This runs when the player picks it up
    public void OnPickup()
    {
        // Find the GameManager and tell it what was picked up
        AudioManager.instance.PlayPickup();
        GameManager.instance.CollectItem(itemType, itemName);
        
        // If this is a key, notify the KeyManager UI (if present)
        if (itemType == ItemType.Key)
        {
            KeyManager km = Object.FindObjectOfType<KeyManager>();
            if (km != null)
                km.PickUpKey();
        }

        // If it is a battery, refill the flashlight
        if (itemType == ItemType.Battery)
        {
            FlashlightController flashlight = Object.FindFirstObjectByType<FlashlightController>();
            if (flashlight != null)
            {
                flashlight.AddBattery(batteryAmount);
            }
        }

        // Destroy the item from the world
        Destroy(gameObject);
    }
}