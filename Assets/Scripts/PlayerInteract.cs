using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Settings")]
    public float interactRange = 5f;
    public KeyManager keyManager;
    public Transform raycastOrigin; // Optional: assign camera if needed
    
    private GameObject currentTarget;
    private Camera playerCamera;

    void Start()
    {
        // Try to find the camera
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;
        Debug.Log("PlayerInteract Started. Camera found: " + (playerCamera != null));   
    }

    void Update()
    {
        // Detect all nearby pickups in a sphere radius
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange);
        
        PickupItem closestPickup = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider col in hits)
        {
            PickupItem pickup = col.GetComponent<PickupItem>();
            if (pickup != null)
            {
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestPickup = pickup;
                }
            }
        }
        
        if (closestPickup != null)
        {
            currentTarget = closestPickup.gameObject;
            
            if (keyManager != null)
                keyManager.ShowInteractionPrompt();
            
            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                closestPickup.OnPickup();
                currentTarget = null;
                if (keyManager != null) keyManager.HideInteractionPrompt();
            }
        }
        else
        {
            ClearTarget();
        }
    }     

    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            if (keyManager != null) keyManager.HideInteractionPrompt();
            currentTarget = null;
        }
    }
}