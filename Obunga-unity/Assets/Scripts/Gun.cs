
using UnityEngine;

public class Gun : MonoBehaviour
{
    float damage = 10f;
    float range = 100f;
    
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
            Debug.Log(hit.transform.name);
        }
    }
}
