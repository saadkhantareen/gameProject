using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public bool requiresKey = false;
    public string requiredKeyName = "SmallKey"; // "SmallKey" or "BasementKey"
    public bool isOpen = false;

    // Simple door open: just rotate it
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isAnimating = false;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
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
        // Snap open for now — we can add animation later
        transform.rotation = openRotation;
        Debug.Log("Door opened!");
    }
}