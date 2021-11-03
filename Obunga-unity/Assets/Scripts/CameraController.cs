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
   /* 
    public float MouseSensitivity;

    private float CameraXRotation;

    public float LerpSpeed;

    private void Update()
   // {
        float MouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        CameraXRotation -= MouseY;
        CameraXRotation = Mathf.Clamp(CameraXRotation, -90f, 90f);

       // transform.localRotation = Quaternion.Euler(CameraXRotation, 0f, 0f);
        transform.parent.Rotate(Vector3.up * MouseX);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(CameraXRotation, 0f, 0f), Time.deltaTime * LerpSpeed);
      //  transform.parent.Rotate(Vector3.Lerp(transform.rotation.eulerAngles, Vector3.up * MouseX, Time.deltaTime * LerpSpeed)); */


   // }

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
       

   }
}