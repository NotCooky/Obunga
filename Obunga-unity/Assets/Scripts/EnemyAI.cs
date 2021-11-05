using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehavior
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    //патрулирование
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //атака
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //состояния
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //проверить на область обнаружения или атаки
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer():
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);
        
        Vector3 distanceToWalkPoint = transform.position = walkPoint;

        //walkpoint достигнута
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint();
    {
        //высчитать случайную позицию в области
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.x + randomZ);
        
        if (Physics.Raycast(walkPoint, -transform.up,))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
       agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
       //убедиться что враг не двигается
       agent.SetDestination(transform.position);

       transform.LookAt(player);

       if (!alreadyAttacked)
       {
          /// <AttackCodeHere>
          /// 
          /// 
          /// 
          /// 
          /// </AttackCodeHere>

          alreadyAttacked = true;
          invoke(nameof(ResetAttack), timeBetweenAttacks
       }

    private void ResetAttack()
    {
       alreadyAttacked = false;
    }
}

