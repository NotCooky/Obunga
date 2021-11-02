using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSnap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.rigidbody != null && !collision.rigidbody.isKinematic)
        {
             collision.transform.position = new Vector3(transform.position.x, collision.transform.position.y, transform.position.z);
        }

       
    }
}
