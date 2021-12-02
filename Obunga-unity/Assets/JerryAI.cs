using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JerryAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask Ground, Player;
    public Vector3 walkPoint;
    public float sightRange;
    public float walkPointRange;
    bool walkPointSet;
    bool playerInSightRange;

    public AudioSource JerryAudio;

    public AudioClip[] SpottedPlayer;


    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, Player);

        if (playerInSightRange)
        {
            FollowPlayer();
        }

        else Patrol();
    }

    void Patrol()
    {
        if (!walkPointSet) SearchForWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint); 

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    void SearchForWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, Ground)) walkPointSet = true;
    }

    void FollowPlayer()
    {
        agent.SetDestination(player.position);
    }
}
