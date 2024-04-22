using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    // Target position for the AI to move towards
    public Transform targetDestination;

    // The current attack target
    private Transform attackTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Start moving the AI Champion to the target destination
        MoveToDestination();
    }

    void Update()
    {
        if (attackTarget != null)
        {
            // Face the attack target
            FaceAttackTarget();
        }
    }

    void MoveToDestination()
    {
        if (targetDestination != null)
        {
            // Set the target destination for the AI Champion
            agent.SetDestination(targetDestination.position);
        }
        else
        {
            Debug.LogError("Target destination not assigned!");
        }
    }

    // Set the attack target for the AI
    public void SetAttackTarget(Transform target)
    {
        attackTarget = target;
    }

    // Rotate to face the attack target
    void FaceAttackTarget()
    {
        if (attackTarget != null)
        {
            Vector3 direction = (attackTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
}
