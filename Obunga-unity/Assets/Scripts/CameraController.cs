using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class CameraController : MonoBehaviour
{

    public float mouseSens;
    public Transform playerBody;
    float xRotation = 0f;
    RaycastHit hit;
    GameObject grabbedObject;
    public Transform grabPos;


   void Start()
   {
       Cursor.lockState = CursorLockMode.Locked;
   }
   void Update()
   {
       float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
       float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
       playerBody.Rotate(Vector3.up * mouseX);

       if(Input.GetMouseButtonDown(1) && Physics.Raycast(transform.position, transform.forward, out hit, 5) && hit.transform.GetComponent<Rigidbody>())
       {
           grabbedObject = hit.transform.gameObject;
       }
       else if(Input.GetMouseButtonUp(1))
       {
           grabbedObject = null;
       } 
       if(grabbedObject)
       {
           grabbedObject.GetComponent<Rigidbody>().velocity = 10 * (grabPos.position - grabbedObject.transform.position);
       }
   }
}