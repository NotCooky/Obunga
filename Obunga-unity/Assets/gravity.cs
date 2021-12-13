using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gravity : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.G))
        {
            Physics.gravity = new Vector3(0, 0, 0);
        }
        else
        {
            Physics.gravity = new Vector3(0, -20f, 0);
        }

        if(Input.GetKey(KeyCode.J))
        {
            Time.timeScale = 0.1f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
