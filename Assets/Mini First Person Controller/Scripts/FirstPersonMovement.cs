using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Jumping")]
    public float jumpForce = 5f;
    public KeyCode jumpKey = KeyCode.Space;
    public LayerMask groundLayer = 1;
    public Transform groundCheck;
    public float groundDistance = 0.4f;

    [Header("Rotation")]
    public float mouseSensitivity = 2f;
    public float bodyRotationSpeed = 10f;

    private bool isGrounded;
    private Rigidbody rb;
    private float yaw = 0f;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Start yaw at current rotation
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        CheckGroundStatus();

        // Mouse X rotates player body
        yaw += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        if (Input.GetKeyDown(jumpKey) && isGrounded)
            Jump();
    }

    void FixedUpdate()
    {
        IsRunning = canRun && Input.GetKey(runningKey);

        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Move relative to player facing direction
        Vector3 forward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
        Vector3 right   = Quaternion.Euler(0f, yaw, 0f) * Vector3.right;

        Vector3 moveDirection = forward * v + right * h;
        moveDirection.y = 0f;

        // Rotate body toward movement direction when strafing
        // Only rotate body for forward/strafe - NOT for backward
        if (moveDirection.sqrMagnitude > 0.01f && v >= 0)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRot,
                Time.fixedDeltaTime * bodyRotationSpeed
            );
            yaw = transform.eulerAngles.y;
        }

        Vector3 targetVelocity = moveDirection * targetMovingSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    void CheckGroundStatus()
    {
        if (groundCheck != null)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}