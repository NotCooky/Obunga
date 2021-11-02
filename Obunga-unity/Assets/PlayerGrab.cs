using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    public GameObject TheCamera;
    public Grabbable Grabbed;
    public float MaxGrabDistance;
    public float ThrowForce;
    public GameObject GrabText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetMouseButtonDown(0))
      {
              if(Grabbed == null)
            {
                if(Physics.Raycast(TheCamera.transform.position, TheCamera.transform.forward, out RaycastHit hit2, MaxGrabDistance))
                {
                    if (hit2.rigidbody != null && hit2.rigidbody.gameObject.GetComponent<Grabbable>())
                    {
                        Grabbed = hit2.rigidbody.gameObject.GetComponent<Grabbable>();
                        hit2.rigidbody.isKinematic = true;
                    }
                }
    
            }
             else 
            {
               Grabbed.GetComponent<Rigidbody>().isKinematic = false; 
               Grabbed = null;
            }
      }

      if(Grabbed != null)
      {
          Grabbed.transform.position = TheCamera.transform.position + TheCamera.transform.forward * MaxGrabDistance;
      }

      if(Input.GetMouseButtonDown(1) && Grabbed != null)
      {
         Grabbed.GetComponent<Rigidbody>().isKinematic = false;  
         Grabbed.GetComponent<Rigidbody>().AddForce(TheCamera.transform.forward * ThrowForce, ForceMode.Impulse); 
         Grabbed = null;
      }

      GrabText.SetActive(Physics.Raycast(TheCamera.transform.position, TheCamera.transform.forward, out RaycastHit hit, MaxGrabDistance) && Grabbed == null &&  hit.rigidbody.gameObject.GetComponent<Grabbable>());




    }



}
