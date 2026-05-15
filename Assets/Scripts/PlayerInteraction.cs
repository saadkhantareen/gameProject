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
                // REPLACE Debug.Log with this:
                UIManager.instance.ShowInteractionPrompt("Press E to pick up " + item.itemName);

                if (Input.GetKeyDown(interactKey))
                {
                    item.OnPickup();
                }
                return; // ADD THIS
            }

            Door door = hit.collider.GetComponent<Door>();
            if (door != null)
            {
                // REPLACE Debug.Log with this:
                UIManager.instance.ShowInteractionPrompt("Press E to open door");
                
                if (Input.GetKeyDown(interactKey))
                {
                    door.TryOpen();
                }
                return; // ADD THIS
            }
        }
        
        // ADD THIS: Hide prompt when not looking at anything
        UIManager.instance.HideInteractionPrompt();
    }
}