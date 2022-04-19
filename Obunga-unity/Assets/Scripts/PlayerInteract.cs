using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Transform playerCam;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(Physics.Raycast(playerCam.position, playerCam.forward, out hit, 10f))
            {
                if(hit.collider.tag == "Interactable")
                {
                    hit.collider.GetComponent<Interactable>().Interact();
                }
            }
        }
    }
}
