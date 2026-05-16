using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 2.5f;
    public KeyCode interactKey = KeyCode.E;
    public Camera playerCamera;

    void Update()
    {
        CheckForInteractable();
    }
    
    void CheckForInteractable()
    {
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            PickupItem item = hit.collider.GetComponent<PickupItem>();

            if (item != null)
            {
                // Show UI prompt if UIManager exists
                if (UIManager.instance != null)
                {
                    UIManager.instance.ShowInteractionPrompt("Press E to pick up " + item.itemName);
                }

                if (Input.GetKeyDown(interactKey))
                {
                    item.OnPickup();
                }
                return;
            }

            Door door = hit.collider.GetComponent<Door>();
            if (door != null)
            {
                // Show UI prompt if UIManager exists
                if (UIManager.instance != null)
                {
                    UIManager.instance.ShowInteractionPrompt("Press E to open door");
                }
                
                if (Input.GetKeyDown(interactKey))
                {
                    door.TryOpen();
                }
                return;
            }
        }
        
        // Hide prompt when not looking at anything
        if (UIManager.instance != null)
        {
            UIManager.instance.HideInteractionPrompt();
        }
    }
}