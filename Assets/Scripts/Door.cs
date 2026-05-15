using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public bool requiresKey = false;
    public string requiredKeyName = "SmallKey"; // "SmallKey" or "BasementKey"
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
        {
            soundEmitter = gameObject.AddComponent<SoundEmitter>();
            soundEmitter.soundRadius = doorNoiseRadius;
        }
    }

    void Update()
    {
        // Smooth door opening animation
        if (isAnimating)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                openRotation,
                Time.deltaTime * openSpeed
            );

            if (Quaternion.Angle(transform.rotation, openRotation) < 1f)
            {
                transform.rotation = openRotation;
                isAnimating = false;
            }
        }
    }

    public void TryOpen()
    {
        if (isOpen) return;

        if (requiresKey)
        {
            bool hasKey = false;
            if (requiredKeyName == "SmallKey")
                hasKey = GameManager.instance.hasSmallKey;
            else if (requiredKeyName == "BasementKey")
                hasKey = GameManager.instance.hasBasementKey;

            if (!hasKey)
            {
                Debug.Log("You need the " + requiredKeyName + " to open this door.");
                return;
            }
        }

        OpenDoor();
    }

    void OpenDoor()
    {
        isOpen = true;
        isAnimating = true;

        // ADD THIS:
        AudioManager.instance.PlayDoor();

        if (makesNoise && soundEmitter != null)
        {
            soundEmitter.EmitSound(transform.position, doorNoiseRadius);
            Debug.Log("Door creaked open! Granny might have heard...");
        }

        Debug.Log("Door opened!");
    }
}