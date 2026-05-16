using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Smoothing")]
    public float rotationSmoothing = 3f; // lower = smoother

    private float currentYaw;

    void Start()
    {
        if (target != null)
            currentYaw = target.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Smooth yaw only - DON'T touch pitch (FirstPersonLook handles it)
        currentYaw = target.eulerAngles.y;

        // Only set yaw - keep pitch from FirstPersonLook untouched
        transform.rotation = Quaternion.Euler(
            transform.localEulerAngles.x,  // pitch from FirstPersonLook
            currentYaw,                     // smooth yaw from player
            0f
        );
    }
}