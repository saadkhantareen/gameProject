using UnityEngine;

[RequireComponent(typeof(SoundEmitter))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Footstep Settings")]
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;
    public float walkNoiseRadius = 5f;
    public float runNoiseRadius = 20f;

    [Header("Detection")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float runSpeedThreshold = 5f;

    private SoundEmitter soundEmitter;
    private Rigidbody rb;
    private float stepTimer = 0f;

    void Start()
    {
        soundEmitter = GetComponent<SoundEmitter>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogError("❌ Rigidbody missing on " + gameObject.name);
        else
            Debug.Log("✅ Rigidbody found on " + gameObject.name);
    }

    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;

        Debug.Log("Speed: " + speed);

        if (speed > 0.1f)
        {
            stepTimer += Time.deltaTime;
            bool isRunning = Input.GetKey(sprintKey) || speed > runSpeedThreshold;
            float currentInterval = isRunning ? runStepInterval : walkStepInterval;
            float currentRadius = isRunning ? runNoiseRadius : walkNoiseRadius;

            if (stepTimer >= currentInterval)
            {
                MakeFootstepSound(currentRadius, isRunning);
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    void MakeFootstepSound(float radius, bool isRunning)
    {
        Debug.Log("👣 Footstep triggered!");

        if (soundEmitter != null)
            soundEmitter.EmitSound(transform.position, radius);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayFootstep();
        else
            Debug.LogError("❌ AudioManager not found!");
    }
}