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

        // IMPORTANT: Prevent Rigidbody physics from interfering with the NavMesh agent or tumbling over
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.isKinematic = true;
        }

        // Ensure NavMeshAgent handles the rotation properly
        agent.updateRotation = true;

        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log(gameObject.name + " found player: " + playerObj.name);
        }
        else
        {
            Debug.LogError(gameObject.name + ": NO PLAYER FOUND! Tag your player as 'Player'!");
        }

        // Get waypoints from container
        if (waypointContainer != null)
        {
            waypoints = new Transform[waypointContainer.childCount];
            for (int i = 0; i < waypointContainer.childCount; i++)
                waypoints[i] = waypointContainer.GetChild(i);
            Debug.Log(gameObject.name + " found " + waypoints.Length + " waypoints!");
        }
        else
        {
            Debug.LogWarning(gameObject.name + ": No waypoint container assigned!");
        }

        agent.speed = patrolSpeed;

        // Force agent onto NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            Debug.Log(gameObject.name + " warped to NavMesh at: " + hit.position);
        }
        else
        {
            Debug.LogError(gameObject.name + " could not find NavMesh nearby! Move it closer to the blue area.");
        }

        GoToNextWaypoint();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null) return;

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(gameObject.name + " is NOT on NavMesh! Position: " + transform.position);
            return;
        }

        // Fix: Ignore the height (Y-axis) so giant players don't confuse the distance check
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
            // ADD THIS: Play footsteps when moving
        if (agent.velocity.magnitude > 0.1f)
        {
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval)
            {
                if (audioSource != null && AudioManager.instance != null)
                {
                    audioSource.PlayOneShot(AudioManager.instance.grannyFootstep);
                }
                footstepTimer = 0f;
            }
        }
    }

    // ─── PATROL ───────────────────────────────────────────────
    void PatrolBehavior(float distanceToPlayer)
    {
        agent.speed = patrolSpeed;

        // If player is close enough → chase immediately
        if (distanceToPlayer <= chaseRange)
        {
            EnterChaseMode();
            return;
        }

        // Keep patrolling waypoints
        if (!agent.pathPending && agent.remainingDistance < 2f)
            GoToNextWaypoint();
    }

    void GoToNextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (!agent.isOnNavMesh) return;

        agent.ResetPath();
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    // ─── CHASE ────────────────────────────────────────────────
    void ChaseBehavior(float distanceToPlayer)
    {
        agent.speed = chaseSpeed;
        
        // Fix: Force the agent to target the ground level of the player, not their floating center 
        Vector3 groundTarget = new Vector3(player.position.x, transform.position.y, player.position.z);
        agent.SetDestination(groundTarget);

        // Attack if close enough
        if (distanceToPlayer <= attackRange)
            AttackPlayer();

        // Lost player → go back to patrol
        if (distanceToPlayer > losePlayerRange)
        {
            currentState = AIState.Patrol;
            agent.speed = patrolSpeed;
            GoToNextWaypoint();
            Debug.Log(gameObject.name + " lost the player!");
        }
    }

    void EnterChaseMode()
    {
        currentState = AIState.Chase;
        agent.speed = chaseSpeed;
        Debug.Log(gameObject.name + " IS CHASING YOU!");
    }

    // ─── SOUND ────────────────────────────────────────────────
    public void HearNoise(Vector3 noisePosition)
    {
        float distance = Vector3.Distance(transform.position, noisePosition);
        if (distance <= hearingRange)
        {
            Debug.Log(gameObject.name + " heard something!");
            EnterChaseMode();
        }
    }

    // ─── ATTACK ───────────────────────────────────────────────
    void AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(damageAmount);
        else
            GameManager.instance.TriggerGameOver();

        Debug.Log(gameObject.name + " CAUGHT YOU!");
    }

    // ─── DEBUG ────────────────────────────────────────────────
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