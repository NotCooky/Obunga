using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    RaycastHit hit;
    GameObject grabOBJ;
    public Transform grabPos;


    // Update is called once per frame
    void Update()
    {
       if (Input.GetMouseButtonDown(1) && Physics.Raycast(transform.position, transform.forward, out hit, 5) && hit.transform.GetComponent<Rigidbody>())
        {
            grabOBJ = hit.transform.gameObject;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            grabOBJ = null;
        }
        if (grabOBJ)
        {
            grabOBJ.GetComponent<Rigidbody>().velocity = 5 * (grabPos.position - grabOBJ.transform.position);
        }
    }



}
