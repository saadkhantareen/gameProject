# Full Source Code — Part 2 (Enemy AI + Player)

## GrannyStyleAI.cs
```csharp
using UnityEngine;
using UnityEngine.AI;

public class GrannyStyleAI : MonoBehaviour
{
    private AudioSource audioSource;
    private float footstepTimer = 0f;
    private float footstepInterval = 0.5f;

    [Header("Detection Settings")]
    public float chaseRange = 50f;
    public float attackRange = 2f;
    public float losePlayerRange = 60f;

    [Header("Movement Settings")]
    public float chaseSpeed = 6f;
    public float patrolSpeed = 1.5f;
    public Transform waypointContainer;

    [Header("Attack Settings")]
    public float attackCooldown = 1f;
    public int damageAmount = 100;

    [Header("Sound Detection")]
    public float hearingRange = 20f;

    private NavMeshAgent agent;
    private Transform player;
    private Transform[] waypoints;
    private int currentWaypoint = 0;

    private enum AIState { Patrol, Chase }
    private AIState currentState = AIState.Patrol;
    private float lastAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) { rb.freezeRotation = true; rb.isKinematic = true; }
        agent.updateRotation = true;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (waypointContainer != null)
        {
            waypoints = new Transform[waypointContainer.childCount];
            for (int i = 0; i < waypointContainer.childCount; i++)
                waypoints[i] = waypointContainer.GetChild(i);
        }

        agent.speed = patrolSpeed;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
            agent.Warp(hit.position);

        GoToNextWaypoint();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;
        if (!agent.isOnNavMesh) return;

        Vector3 flatPlayerPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        float distanceToPlayer = Vector3.Distance(transform.position, flatPlayerPosition);

        switch (currentState)
        {
            case AIState.Patrol: PatrolBehavior(distanceToPlayer); break;
            case AIState.Chase: ChaseBehavior(distanceToPlayer); break;
        }

        if (agent.velocity.magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                if (audioSource != null && AudioManager.instance != null)
                    audioSource.PlayOneShot(AudioManager.instance.grannyFootstep);
                footstepTimer = 0f;
            }
        }
    }

    void PatrolBehavior(float distanceToPlayer)
    {
        agent.speed = patrolSpeed;
        if (distanceToPlayer <= chaseRange) { EnterChaseMode(); return; }
        if (!agent.pathPending && agent.remainingDistance < 2f) GoToNextWaypoint();
    }

    void GoToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (!agent.isOnNavMesh) return;
        agent.ResetPath();
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void ChaseBehavior(float distanceToPlayer)
    {
        agent.speed = chaseSpeed;
        Vector3 groundTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
        agent.SetDestination(groundTarget);
        if (distanceToPlayer <= attackRange) AttackPlayer();
        if (distanceToPlayer > losePlayerRange)
        {
            currentState = AIState.Patrol;
            agent.speed = patrolSpeed;
            GoToNextWaypoint();
        }
    }

    void EnterChaseMode() { currentState = AIState.Chase; agent.speed = chaseSpeed; }

    public void HearNoise(Vector3 noisePosition)
    {
        float distance = Vector3.Distance(transform.position, noisePosition);
        if (distance <= hearingRange) EnterChaseMode();
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null) playerHealth.TakeDamage(damageAmount);
        else GameManager.instance.TriggerGameOver();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue; Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
}
```

## PlayerHealth.cs
```csharp
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Damage Effects")]
    public float damageScreenFlashDuration = 0.3f;
    public Color damageColor = new Color(1f, 0f, 0f, 0.3f);

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UIManager.instance.UpdateHealth(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        UIManager.instance.UpdateHealth(currentHealth, maxHealth);
        StartCoroutine(DamageFlash());
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        AudioManager.instance.PlayDeath();
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script.GetType().Name.Contains("Movement") || 
                script.GetType().Name.Contains("Controller") ||
                script.GetType().Name.Contains("Player"))
                script.enabled = false;
        }
        GameManager.instance.TriggerGameOver();
    }

    System.Collections.IEnumerator DamageFlash()
    { yield return new WaitForSeconds(damageScreenFlashDuration); }

    public void Heal(int amount)
    { currentHealth = Mathf.Min(currentHealth + amount, maxHealth); }

    public float GetHealthPercent() { return (float)currentHealth / maxHealth; }
}
```

## FlashlightController.cs
```csharp
using UnityEngine;
using UnityEngine.UI;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public float maxBattery = 100f;
    public float drainRate = 2f;
    public KeyCode toggleKey = KeyCode.F;

    [Header("References")]
    public Light flashlight;

    private float currentBattery;
    private bool isOn = true;

    void Start()
    {
        currentBattery = maxBattery;
        flashlight.enabled = true;
        if (UIManager.instance != null) UIManager.instance.UpdateBattery(currentBattery, maxBattery);
    }

    void Update()
    {
        HandleToggle();
        HandleBatteryDrain();
        if (UIManager.instance != null) UIManager.instance.UpdateBattery(currentBattery, maxBattery);
    }

    void HandleToggle()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
            if (AudioManager.instance != null) AudioManager.instance.PlayFlashlightClick();
        }
    }

    void HandleBatteryDrain()
    {
        if (isOn && currentBattery > 0)
        {
            currentBattery -= drainRate * Time.deltaTime;
            float batteryPercent = currentBattery / maxBattery;
            flashlight.intensity = batteryPercent * 3f;
            if (currentBattery <= 0)
            { currentBattery = 0; isOn = false; flashlight.enabled = false; }
        }
    }

    public void AddBattery(float amount)
    {
        currentBattery = Mathf.Min(currentBattery + amount, maxBattery);
        if (currentBattery > 0 && !isOn) { isOn = true; flashlight.enabled = true; }
    }

    public float GetBatteryPercent() { return currentBattery / maxBattery; }
}
```

## PlayerFootsteps.cs
```csharp
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
    }

    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;
        if (speed > 0.1f)
        {
            stepTimer += Time.deltaTime;
            bool isRunning = Input.GetKey(sprintKey) || speed > runSpeedThreshold;
            float currentInterval = isRunning ? runStepInterval : walkStepInterval;
            float currentRadius = isRunning ? runNoiseRadius : walkNoiseRadius;
            if (stepTimer >= currentInterval)
            { MakeFootstepSound(currentRadius, isRunning); stepTimer = 0f; }
        }
        else stepTimer = 0f;
    }

    void MakeFootstepSound(float radius, bool isRunning)
    {
        if (soundEmitter != null) soundEmitter.EmitSound(transform.position, radius);
        if (AudioManager.instance != null) AudioManager.instance.PlayFootstep();
    }
}
```
