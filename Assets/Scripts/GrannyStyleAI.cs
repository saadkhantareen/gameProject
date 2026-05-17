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

    // ── ADD THIS ──
    private Animator animator;

    private enum AIState { Patrol, Chase }
    private AIState currentState = AIState.Patrol;

    private float lastAttackTime = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // ── ADD THIS ──
        animator = GetComponent<Animator>();
        Debug.Log("Animator found: " + (animator != null)); // ← add this

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.isKinematic = true;
        }

        agent.updateRotation = true;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log(gameObject.name + " found player: " + playerObj.name);
        }
        else
        {
            Debug.LogError(gameObject.name + ": NO PLAYER FOUND!");
        }

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
        Debug.Log("isOnNavMesh: " + agent.isOnNavMesh);
        Debug.Log("velocity: " + agent.velocity.magnitude);
        if (player == null) return;
        if (!agent.isOnNavMesh) return;

        Vector3 flatPlayerPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        float distanceToPlayer = Vector3.Distance(transform.position, flatPlayerPosition);

        switch (currentState)
        {
            case AIState.Patrol:
                PatrolBehavior(distanceToPlayer);
                break;
            case AIState.Chase:
                ChaseBehavior(distanceToPlayer);
                break;
        }

        // ── ADD THIS — Update animation speed ──
        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
            Debug.Log("Speed: " + speed); 
        }

        // Footsteps
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

        if (distanceToPlayer <= chaseRange)
        {
            EnterChaseMode();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 2f)
            GoToNextWaypoint();
    }

    void GoToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (!agent.isOnNavMesh) return;

        agent.ResetPath();
        agent.SetDestination(waypoints[currentWaypoint].position);
        Debug.Log("Going to waypoint: " + waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void ChaseBehavior(float distanceToPlayer)
    {
        agent.speed = chaseSpeed;

        Vector3 groundTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
        agent.SetDestination(groundTarget);

        if (distanceToPlayer <= attackRange)
            AttackPlayer();

        if (distanceToPlayer > losePlayerRange)
        {
            currentState = AIState.Patrol;
            agent.speed = patrolSpeed;
            GoToNextWaypoint();
        }
    }

    void EnterChaseMode()
    {
        currentState = AIState.Chase;
        agent.speed = chaseSpeed;

        // ── ADD THIS ──
        if (animator != null)
            animator.SetBool("IsAttacking", false);
    }

    public void HearNoise(Vector3 noisePosition)
    {
        float distance = Vector3.Distance(transform.position, noisePosition);
        if (distance <= hearingRange)
            EnterChaseMode();
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        // ── ADD THIS ──
        if (animator != null)
            animator.SetBool("IsAttacking", true);

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(damageAmount);
        else
            GameManager.instance.TriggerGameOver();

        Debug.Log(gameObject.name + " CAUGHT YOU!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
}