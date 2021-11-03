
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float range = 100f;
    public float shootStrength;
    
    public Camera playerCam;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {
            hit.rigidbody.AddForce(playerCam.transform.forward * shootStrength);
        }
    }
}
