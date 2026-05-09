using UnityEngine;
using UnityEngine.AI;

public class GhostAI : MonoBehaviour
{
    [Header("Detection")]
    public float chaseRange = 15f;
    public float attackRange = 1.5f;

    [Header("Speed")]
    public float wanderSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Waypoints")]
    public Transform waypointContainer;

    private NavMeshAgent agent;
    private Transform player;
    private Transform[] waypoints;
    private int currentWaypoint = 0;

    private enum State { Wandering, Chasing, Attacking }
    private State currentState = State.Wandering;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Get all waypoints from container
        waypoints = new Transform[waypointContainer.childCount];
        for (int i = 0; i < waypointContainer.childCount; i++)
        {
            waypoints[i] = waypointContainer.GetChild(i);
        }

        GoToNextWaypoint();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Wandering:
                WanderUpdate();
                // Check if player is close enough to chase
                if (distanceToPlayer <= chaseRange)
                {
                    currentState = State.Chasing;
                    agent.speed = chaseSpeed;
                    Debug.Log("Ghost is chasing you!");
                }
                break;

            case State.Chasing:
                // Always move toward player
                agent.SetDestination(player.position);

                // Close enough to attack
                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attacking;
                    agent.SetDestination(transform.position);
                    Debug.Log("Ghost caught you!");
                    // Connect to GameManager later
                    // GameManager.instance.TriggerGameOver();
                }

                // Player ran away
                if (distanceToPlayer > chaseRange * 1.5f)
                {
                    currentState = State.Wandering;
                    agent.speed = wanderSpeed;
                    GoToNextWaypoint();
                    Debug.Log("Ghost lost you.");
                }
                break;

            case State.Attacking:
                // Face the player
                transform.LookAt(player);
                break;
        }
    }

    void WanderUpdate()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextWaypoint();
        }
    }

    void GoToNextWaypoint()
    {
        if (waypoints.Length == 0) return;
        agent.speed = wanderSpeed;
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    // Draw chase range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}