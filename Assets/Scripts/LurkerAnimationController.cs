using UnityEngine;
using UnityEngine.AI;

public class LurkerAnimationController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Always chase player
        if (player != null)
        {
            agent.SetDestination(player.position);
        }

        // Speed controls walk animation
        float speed = agent.velocity.magnitude;
        animator.SetBool("isWalking", speed > 0.1f);
    }

    public void TriggerAttack()
    {
        animator.SetBool("isAttacking", true);
    }

    public void StopAttack()
    {
        animator.SetBool("isAttacking", false);
    }

    public void Die()
    {
        animator.SetBool("isDead", true);
    }
}