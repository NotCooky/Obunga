using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    public Transform camPos;
    public V3PIDController piddy;
    public Transform targetPos;
    Rigidbody grabbedObj;
    public float ForceMultiplier;

    void Update()
    {
        RaycastHit hit;

        if (Input.GetMouseButtonDown(1) && Physics.Raycast(camPos.transform.position, camPos.transform.forward, out hit, 3f) && hit.rigidbody != null)
        {
            grabbedObj = hit.rigidbody;
            Debug.Log("grabbed object");
        }
        else if (Input.GetMouseButtonUp(1))
        {
            grabbedObj = null;
            Debug.Log("Dropped object");
        }

        if (grabbedObj)
        {
            Vector3 Error = targetPos.position - grabbedObj.transform.position;
            grabbedObj.AddForce(piddy.GetOutput(Error) * ForceMultiplier);
        }
    }
}