using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIStates
{
    WANDER,
    PURSUE,
    ATTACK,
    RECOVERY
}

public class AI : MonoBehaviour
{
    [SerializeField] AIStates currentState;
    [SerializeField] Transform enemyLocation;
    [SerializeField] float wanderRange;
    [SerializeField] float pursueRange = 5.0f;
    [SerializeField] float attackRange = 5.0f;
    [SerializeField] float attackForce;
    float enemyDistance;
    Vector3 startingLocation;
    NavMeshAgent agent;
    float wanderTime = 5.0f;
    float recoveryTime = 2.0f;
    float wanderTimer = 0.0f;
    float recoveryTimer = 0.0f;
    Rigidbody rb;
    

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        startingLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        enemyDistance = Vector3.Distance(enemyLocation.position, transform.position);

        switch (currentState)
        {
            case AIStates.WANDER:
                UpdateWander();
                break;
            case AIStates.PURSUE:
                UpdatePursue();
                break;
            case AIStates.ATTACK:
                UpdateAttack();
                break;
            case AIStates.RECOVERY:
                UpdateRecovery();
                break;
            default:
                Debug.Log("Something bad has happened");
                break;

        }
    }

    void UpdateWander()
    {

        if (wanderTimer >= wanderTime)
        {
            agent.SetDestination(GetRandomPointInRange());
            wanderTimer = 0.0f;
        }
        else
        {
            wanderTimer += 1.0f * Time.deltaTime;
        }

        if (enemyDistance <= pursueRange)
        {
            currentState = AIStates.PURSUE;
        }
    }

    void UpdatePursue()
    {
        agent.SetDestination(enemyLocation.position);
        
        if (enemyDistance <= attackRange)
        {
            agent.enabled = false;
            rb.isKinematic = false;
            currentState = AIStates.ATTACK;
        }
        else if (enemyDistance > pursueRange)
        {
            currentState = AIStates.WANDER;
        }
    }

    void UpdateAttack()
    {

        rb.AddForce((enemyLocation.position - transform.position) * attackForce, ForceMode.Impulse);
        currentState = AIStates.RECOVERY;

    }

    void UpdateRecovery() 
    {
        
        if (recoveryTimer >= recoveryTime)
        {
            agent.enabled = true;
            rb.isKinematic = true;
            recoveryTimer = 0.0f;
            currentState = AIStates.PURSUE;
        }
        else
        {
            recoveryTimer += 1.0f * Time.deltaTime;
        }

    }

    Vector3 GetRandomPointInRange()
    {
        Vector3 offset = new Vector3(Random.Range(-wanderRange, wanderRange), 0, Random.Range(-wanderRange, wanderRange));

        NavMeshHit hit;

        bool gotPoint = NavMesh.SamplePosition(startingLocation + offset, out hit, 1, NavMesh.AllAreas);

        if (gotPoint)
        {
            return hit.position;
        }

        return Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player was hit!");
    }

}
