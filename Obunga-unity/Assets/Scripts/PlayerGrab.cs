using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    Rigidbody grabbedObj;
    public LineRenderer lr;
    public Transform camPos;
    public Transform targetPos;
    public V3PIDController piddy;
    public float ForceMultiplier;
    public float throwingForce;

    void Start()
    {
        lr.positionCount = 2;
    }

    void Update()
    {
        RaycastHit hit;

        if (Input.GetMouseButtonDown(1) && Physics.Raycast(camPos.transform.position, camPos.transform.forward, out hit, 3f) && hit.rigidbody != null)
        {
            grabbedObj = hit.rigidbody;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            grabbedObj = null;
            lr.enabled = false;
        }

        if (grabbedObj)
        {
            Vector3 Error = targetPos.position - grabbedObj.transform.position;
            grabbedObj.AddForce(piddy.GetOutput(Error) * ForceMultiplier);

            lr.enabled = true;
            lr.SetPosition(0, targetPos.position);
            lr.SetPosition(1, grabbedObj.position);

            if(Input.GetMouseButtonDown(0))
            {
                grabbedObj.AddForce(camPos.transform.forward * throwingForce, ForceMode.Impulse);
                grabbedObj = null;
                lr.enabled = false;
            }
        }
    }
}