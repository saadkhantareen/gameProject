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
                    if (keyManager != null) keyManager.PickUpKey();
                    Destroy(currentTarget);
                    currentTarget = null;
                    if (keyManager != null) keyManager.HideInteractionPrompt();
                }
            }
            else ClearTarget();
        }
        else ClearTarget();
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
