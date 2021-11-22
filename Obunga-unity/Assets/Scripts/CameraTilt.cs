using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTilt : MonoBehaviour
{
    Quaternion tilt = Quaternion.Euler(0, 0, 15);
    Quaternion otherTilt = Quaternion.Euler(0, 0, -15);
 
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        { 
            transform.rotation = Quaternion.Slerp(transform.rotation, tilt, Time.deltaTime);
        }
        else if(Input.GetKeyUp(KeyCode.A))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, otherTilt, Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, otherTilt, Time.deltaTime);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, tilt, Time.deltaTime);
        }
    }
}
