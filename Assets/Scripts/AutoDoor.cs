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
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("No Player tag found!");
    }

    void Update()
    {
        bool shouldOpen = false;

        // Check distance to player
        if (player != null && Vector3.Distance(transform.position, player.position) <= triggerDistance)
        {
            shouldOpen = true;
        }

        // Check distance to any enemy
        if (!shouldOpen)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                if (enemy != null && Vector3.Distance(transform.position, enemy.transform.position) <= triggerDistance)
                {
                    shouldOpen = true;
                    break;
                }
            }
        }

        if (shouldOpen)
        {
            doorTransform.localRotation = Quaternion.Slerp(
                doorTransform.localRotation,
                openRotation,
                Time.deltaTime * openSpeed
            );
        }
        else
        {
            doorTransform.localRotation = Quaternion.Slerp(
                doorTransform.localRotation,
                closedRotation,
                Time.deltaTime * openSpeed
            );
        }
    }
}