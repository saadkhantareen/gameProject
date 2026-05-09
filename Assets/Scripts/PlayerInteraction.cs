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
        // Shoot a ray from the center of the screen forward
        Ray ray = playerCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2, Screen.height / 2, 0));

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // Check if the object has a PickupItem component
            PickupItem item = hit.collider.GetComponent<PickupItem>();

            if (item != null)
            {
                // We will show UI hint here later (Phase 8)
                Debug.Log("Press E to pick up " + item.itemName);

                if (Input.GetKeyDown(interactKey))
                {
                    item.OnPickup();
                }
            }

            // Check if it has a Door component
            Door door = hit.collider.GetComponent<Door>();
            if (door != null)
            {
                Debug.Log("Press E to open door");
                if (Input.GetKeyDown(interactKey))
                {
                    door.TryOpen();
                }
            }
        }
    }
}