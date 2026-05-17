using UnityEngine;
using UnityEngine.AI;

public class LurkerAnimationController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Speed controls Idle/Walk/Run
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }

    // Call when lurker attacks player
    public void TriggerAttack()
    {
        animator.SetBool("IsAttacking", true);
    }

    // Call when lurker stops attacking
    public void StopAttack()
    {
        animator.SetBool("IsAttacking", false);
    }
}