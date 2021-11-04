
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float range = 100f;
    public float shootStrength;
    public ParticleSystem muzzleFlash;
    public Animation shootAnim;
    
    public Camera playerCam;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        

        if(Input.GetMouseButton(0))
        {
            muzzleFlash.Play();
            shootAnim.Play();
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
