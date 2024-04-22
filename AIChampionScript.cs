using UnityEngine;
using UnityEngine.AI;

public class AIChampionScript : MonoBehaviour
{
    public float health = 500;
    public float attackRange = 7f;
    public float attackCooldown = 2f;

    GameObject target;
    bool hasTarget = false;

    NavMeshAgent myNavMeshAgent;

    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();

        InvokeRepeating(nameof(FindAndAttackEnemy), 0f, 1f);
    }

    void Update()
    {
        ManageHealth();

        if (hasTarget)
        {
            AttackTargetInRange();
        }
        else
        {
            myNavMeshAgent.isStopped = false; // Resume movement if no target
        }
    }

    void FindAndAttackEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("BlueMinion"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hitCollider.gameObject;
                }
            }
            else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("BlueTower"))
            {
                TowerAIScript tower = hitCollider.gameObject.GetComponent<TowerAIScript>();
                if (tower != null && tower.isBlue)
                {
                    tower.health -= 20;
                    Debug.Log("AI attacked tower! Tower health: " + tower.health);
                }
            }
            else if (hitCollider.gameObject.layer == LayerMask.NameToLayer("BluePlayer"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = hitCollider.gameObject;
                }
            }
        }

        if (closestEnemy != null)
        {
            target = closestEnemy;
            hasTarget = true;
            myNavMeshAgent.isStopped = false; // Ensure AI is moving towards the target
        }
        else
        {
            hasTarget = false;
        }    
    }


    void AttackTargetInRange()
    {
        if (Time.time >= attackCooldown && hasTarget && Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            DealDamageToTarget();
            attackCooldown = Time.time + attackCooldown;
        }

        // Rotate towards the target while attacking
        if (target != null)
        {
            myNavMeshAgent.SetDestination(target.transform.position);
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    void DealDamageToTarget()
    {
        if (target != null)
        {
            if (target.layer == LayerMask.NameToLayer("BlueMinion"))
            {
                target.GetComponent<MinionAIScript>().health -= 20;
                Debug.Log("AI attacked minion! Minion health: " + target.GetComponent<MinionAIScript>().health);
            }
            else if (target.layer == LayerMask.NameToLayer("BluePlayer"))
            {
                target.GetComponent<PlayerScript1>().health -= 20;
                Debug.Log("AI attacked player! Player health: " + target.GetComponent<PlayerScript1>().health);
            }
        }
    }

    void ManageHealth()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            Debug.Log("AI Champion Destroyed!");
        }
    }
}