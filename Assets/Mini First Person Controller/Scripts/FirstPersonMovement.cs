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
    public LayerMask groundLayer; // Set this to 'Default' or 'Floor' in Inspector
    public Transform groundCheck; // Create an empty child at player's feet
    public float groundDistance = 0.2f;

    private bool isGrounded;
    Rigidbody rigidbody;
    
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Jump input should be checked in Update for better responsiveness
        CheckGroundStatus();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        IsRunning = canRun && Input.GetKey(runningKey);

        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement while preserving current Y velocity (gravity/jumping)
        rigidbody.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.linearVelocity.y, targetVelocity.y);
    }

    void CheckGroundStatus()
    {
        // Creates a tiny invisible sphere at the player's feet to see if it hits the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
    }

    void Jump()
    {
        // Reset Y velocity before jumping so double-taps don't launch you to space
        rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}