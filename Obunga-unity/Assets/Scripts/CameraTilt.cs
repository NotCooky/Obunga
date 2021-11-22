using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    public float tiltAmount = 15f;
 
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, tiltAmount), Time.deltaTime);
        }
        
        if(Input.GetKeyUp(KeyCode.A))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, -tiltAmount), Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, -tiltAmount), Time.deltaTime);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, tiltAmount), Time.deltaTime);
        }
    }
}
