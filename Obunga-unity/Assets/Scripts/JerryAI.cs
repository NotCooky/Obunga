using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JerryAI : MonoBehaviour
{
    bool playerDetected;
    public Transform player;
    public Collider detectionTrigger;
    public AudioSource JerryAudio;
    public AudioClip[] OnDetectSFX;
    public AudioClip[] OnLostSFX;
    public Vector3 walkPoint;
    Rigidbody rb;

    void Start()
    {
        playerDetected = false;
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (playerDetected)
        {
            FollowPlayer();
        }
        else return;
    }

    void OnTriggerEnter(Collider detectionTrigger)
    {
        if (detectionTrigger.tag == "Player")
        {
            playerDetected = true;
            JerryAudio.PlayOneShot(OnDetectSFX[Random.Range(0, OnDetectSFX.Length)]);
        }
        else return;
    }

    void OnTriggerExit(Collider detectionTrigger)
    {
        playerDetected = false;
        JerryAudio.PlayOneShot(OnLostSFX[Random.Range(0, OnLostSFX.Length)]);
    }

    void FollowPlayer()
    { 
        rb.AddForce((player.position - transform.position).normalized);  
    }
}
