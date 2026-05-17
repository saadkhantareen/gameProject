using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    [Header("Touch Look")]
    public bool useTouchLook = true;
    public float touchSensitivity = 0.08f;
    public bool allowMouseLook = false;
    public bool mouseDragToLook = true;
    [Header("Cursor")]
    public bool lockCursor = false;

    public Vector2 LookDelta { get; private set; }

    Vector2 velocity;
    Vector2 frameVelocity;

    void Start()
    {
        if (lockCursor && !Application.isMobilePlatform)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        Vector2 lookDelta = Vector2.zero;

        if (useTouchLook && Application.isMobilePlatform && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                lookDelta = touch.deltaPosition * touchSensitivity;
            }
        }
        else if (mouseDragToLook && Input.GetMouseButton(0))
        {
            lookDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * sensitivity;
        }
        else if (allowMouseLook)
        {
            lookDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * sensitivity;
        }

        Vector2 rawFrameVelocity = lookDelta;
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        LookDelta = frameVelocity;
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        // Pitch up/down on camera
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
    }
}