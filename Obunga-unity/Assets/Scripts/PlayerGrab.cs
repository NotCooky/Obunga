using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    public Transform camPos;
    public V3PIDController piddy;
    public Transform targetPos;
    public Rigidbody grabbedObj;
    public float ForceMultiplier;
    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(camPos.transform.position, camPos.transform.forward, out hit, 3f))
        {
            if (hit.rigidbody && Input.GetMouseButtonDown(1))
            {
                grabbedObj = hit.rigidbody;
            }
            else if(Input.GetMouseButtonUp(1))
            {
                grabbedObj = null;
            }
        }
        else
        {
            grabbedObj = null;
        }

        if (grabbedObj != null)
        {
            Vector3 Error = targetPos.position - grabbedObj.transform.position;
            grabbedObj.AddForce(piddy.GetOutput(Error) * ForceMultiplier);
        }
    }
}