# Full Source Code — Part 3 (Items, Doors, Interaction, Movement)

## PickupItem.cs
```csharp
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum ItemType { Battery, Candle, Key, JournalPage }

    [Header("Item Settings")]
    public ItemType itemType;
    public float batteryAmount = 30f;
    public string itemName = "Item";

    [Header("Sound Settings")]
    public bool makesNoiseWhenDropped = true;
    public float dropNoiseRadius = 15f;

    private SoundEmitter soundEmitter;
    private Rigidbody rb;

    void Start()
    {
        if (makesNoiseWhenDropped)
        { soundEmitter = gameObject.AddComponent<SoundEmitter>(); soundEmitter.soundRadius = dropNoiseRadius; }
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (makesNoiseWhenDropped && rb != null && !rb.isKinematic)
            if (collision.relativeVelocity.magnitude > 2f)
                if (soundEmitter != null) soundEmitter.EmitSound(transform.position, dropNoiseRadius);
    }

    public void OnPickup()
    {
        AudioManager.instance.PlayPickup();
        GameManager.instance.CollectItem(itemType, itemName);
        if (itemType == ItemType.Key)
        { KeyManager km = Object.FindObjectOfType<KeyManager>(); if (km != null) km.PickUpKey(); }
        if (itemType == ItemType.Battery)
        {
            FlashlightController flashlight = Object.FindFirstObjectByType<FlashlightController>();
            if (flashlight != null) flashlight.AddBattery(batteryAmount);
        }
        Destroy(gameObject);
    }
}
```

## Door.cs
```csharp
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public bool requiresKey = false;
    public string requiredKeyName = "SmallKey";
    public bool isOpen = false;
    public float openSpeed = 2f;

    [Header("Sound Settings")]
    public bool makesNoise = true;
    public float doorNoiseRadius = 20f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isAnimating = false;
    private SoundEmitter soundEmitter;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
        if (makesNoise)
        { soundEmitter = gameObject.AddComponent<SoundEmitter>(); soundEmitter.soundRadius = doorNoiseRadius; }
    }

    void Update()
    {
        if (isAnimating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
            if (Quaternion.Angle(transform.rotation, openRotation) < 1f)
            { transform.rotation = openRotation; isAnimating = false; }
        }
    }

    public void TryOpen()
    {
        if (isOpen) return;
        if (requiresKey)
        {
            bool hasKey = false;
            if (requiredKeyName == "SmallKey") hasKey = GameManager.instance.hasSmallKey;
            else if (requiredKeyName == "BasementKey") hasKey = GameManager.instance.hasBasementKey;
            if (!hasKey) return;
        }
        OpenDoor();
    }

    void OpenDoor()
    {
        isOpen = true; isAnimating = true;
        AudioManager.instance.PlayDoor();
        if (makesNoise && soundEmitter != null)
            soundEmitter.EmitSound(transform.position, doorNoiseRadius);
    }
}
```

## AutoDoor.cs
```csharp
using UnityEngine;

public class AutoDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorTransform;
    public float openAngle = -180f;
    public float openSpeed = 20f;
    public float triggerDistance = 3f;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Transform player;

    void Start()
    {
        closedRotation = doorTransform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        bool shouldOpen = false;
        if (player != null && Vector3.Distance(transform.position, player.position) <= triggerDistance)
            shouldOpen = true;
        if (!shouldOpen)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
                if (enemy != null && Vector3.Distance(transform.position, enemy.transform.position) <= triggerDistance)
                { shouldOpen = true; break; }
        }
        Quaternion target = shouldOpen ? openRotation : closedRotation;
        doorTransform.localRotation = Quaternion.Slerp(doorTransform.localRotation, target, Time.deltaTime * openSpeed);
    }
}
```

## ExitDoor.cs
```csharp
using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [Header("Exit Settings")]
    public bool requiresAllCandles = false;
    public bool requiresKey = true;
    public string requiredKeyName = "BasementKey";

    [Header("Runtime References")]
    public Collider blockingCollider;

    private bool playerInZone = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            CheckExitConditions();
            if (CanEscape()) GameManager.instance.TriggerWin();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        { playerInZone = false; UIManager.instance.HideInteractionPrompt(); }
    }

    void Update()
    { if (playerInZone && Input.GetKeyDown(KeyCode.E)) TryEscape(); }

    void CheckExitConditions()
    {
        if (CanEscape())
        { UIManager.instance.ShowInteractionPrompt("Press E to ESCAPE!"); return; }
        if (requiresAllCandles && GameManager.instance.candlesCollected < GameManager.instance.candlesNeeded)
        {
            int remaining = GameManager.instance.candlesNeeded - GameManager.instance.candlesCollected;
            UIManager.instance.ShowInteractionPrompt("Door won't open — find " + remaining + " more candle(s)");
            return;
        }
        if (requiresKey)
        {
            bool hasKey = requiredKeyName == "BasementKey" 
                ? GameManager.instance.hasBasementKey : GameManager.instance.hasSmallKey;
            if (!hasKey)
            { UIManager.instance.ShowInteractionPrompt("Door won't open — find the key to escape."); return; }
        }
        UIManager.instance.ShowInteractionPrompt("Door is locked.");
    }

    void TryEscape()
    {
        if (CanEscape())
        {
            if (blockingCollider != null) blockingCollider.enabled = false;
            GameManager.instance.TriggerWin();
        }
    }

    bool CanEscape()
    {
        if (requiresAllCandles && GameManager.instance.candlesCollected < GameManager.instance.candlesNeeded) return false;
        if (requiresKey)
        {
            bool hasKey = requiredKeyName == "BasementKey" 
                ? GameManager.instance.hasBasementKey : GameManager.instance.hasSmallKey;
            if (!hasKey) return false;
        }
        return true;
    }
}
```

## PlayerInteraction.cs
```csharp
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 2.5f;
    public KeyCode interactKey = KeyCode.E;
    public Camera playerCamera;

    void Update() { CheckForInteractable(); }
    
    void CheckForInteractable()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange))
        {
            PickupItem item = hit.collider.GetComponent<PickupItem>();
            if (item != null)
            {
                if (UIManager.instance != null)
                    UIManager.instance.ShowInteractionPrompt("Press E to pick up " + item.itemName);
                if (Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(0)) item.OnPickup();
                return;
            }
            Door door = hit.collider.GetComponent<Door>();
            if (door != null)
            {
                if (UIManager.instance != null)
                    UIManager.instance.ShowInteractionPrompt("Press E to open door");
                if (Input.GetKeyDown(interactKey) || Input.GetMouseButtonDown(0)) door.TryOpen();
                return;
            }
        }
        if (UIManager.instance != null) UIManager.instance.HideInteractionPrompt();
    }
}
```

## SoundEmitter.cs
```csharp
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    [Header("Sound Settings")]
    public float soundRadius = 15f;
    public bool debugMode = false;

    public void EmitSound(Vector3 soundPosition, float radiusOverride = -1f)
    {
        float actualRadius = radiusOverride > 0 ? radiusOverride : soundRadius;
        GrannyStyleAI[] enemies = Object.FindObjectsByType<GrannyStyleAI>(FindObjectsSortMode.None);
        foreach (GrannyStyleAI enemy in enemies) enemy.HearNoise(soundPosition);
    }

    public void EmitSoundHere() { EmitSound(transform.position); }
}
```

## KeyManager.cs
```csharp
using UnityEngine;
using TMPro;

public class KeyManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI keysCounterText;
    public TextMeshProUGUI interactionPromptText;
    public TextMeshProUGUI notificationText;

    private int keysCollected = 0;
    private int totalKeys = 1;

    void Start()
    {
        UpdateKeyUI();
        interactionPromptText.gameObject.SetActive(false);
        notificationText.gameObject.SetActive(false);
    }

    public void PickUpKey()
    { keysCollected++; UpdateKeyUI(); ShowNotification("Picked up the key!\nNow go to the Door outside the house to escape."); }

    private void UpdateKeyUI() { keysCounterText.text = $"keys: {keysCollected}/{totalKeys}"; }
    public void ShowInteractionPrompt() { interactionPromptText.gameObject.SetActive(true); }
    public void HideInteractionPrompt() { interactionPromptText.gameObject.SetActive(false); }

    public void ShowNotification(string message)
    { notificationText.text = message; notificationText.gameObject.SetActive(true); Invoke(nameof(HideNotification), 5f); }

    private void HideNotification() { notificationText.gameObject.SetActive(false); }
}
```

## MobileMovements.cs
```csharp
using UnityEngine;

public class MobileMoveButtons : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    public void PressUp()    { MoveInput = new Vector2(MoveInput.x, 1f); }
    public void ReleaseUp()  { if (MoveInput.y > 0) MoveInput = new Vector2(MoveInput.x, 0f); }
    public void PressDown()  { MoveInput = new Vector2(MoveInput.x, -1f); }
    public void ReleaseDown(){ if (MoveInput.y < 0) MoveInput = new Vector2(MoveInput.x, 0f); }
    public void PressLeft()  { MoveInput = new Vector2(-1f, MoveInput.y); }
    public void ReleaseLeft(){ if (MoveInput.x < 0) MoveInput = new Vector2(0f, MoveInput.y); }
    public void PressRight() { MoveInput = new Vector2(1f, MoveInput.y); }
    public void ReleaseRight(){ if (MoveInput.x > 0) MoveInput = new Vector2(0f, MoveInput.y); }
}
```

## PlayerInteract.cs (Legacy)
```csharp
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 3f;
    public KeyManager keyManager;
    private GameObject currentTarget;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Key"))
            {
                currentTarget = hit.collider.gameObject;
                if (keyManager != null) keyManager.ShowInteractionPrompt();
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
                {
                    PickupItem pickup = currentTarget.GetComponent<PickupItem>();
                    if (pickup != null) pickup.OnPickup();
                    else { if (keyManager != null) keyManager.PickUpKey(); Destroy(currentTarget); }
                    currentTarget = null;
                    if (keyManager != null) keyManager.HideInteractionPrompt();
                }
            }
            else ClearTarget();
        }
        else ClearTarget();
    }

    private void ClearTarget()
    { if (currentTarget != null) { if (keyManager != null) keyManager.HideInteractionPrompt(); currentTarget = null; } }
}
```
