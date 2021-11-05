
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float range = 100f;
    public float shootStrength;
    public int maxAmmo = 7;
    private int currentAmmo;
    public float reloadTime;
    private bool isReloading;
    public float fireRate;

    public ParticleSystem muzzleFlash;
    public Animator animator;
    
    public Camera playerCam;

    private float nextTimeToFire = 0f;

    void Start()
    {
       currentAmmo = maxAmmo;
    }

   
    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isReloading)
        {
            return;
        }

        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            
            
        }
    }

      IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Realod");
        animator.SetBool("reloading", true);

        yield return new WaitForSeconds(reloadTime - .25f);
        animator.SetBool("reloading", false);
        yield return new WaitForSeconds(.25f);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        currentAmmo--;
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {
            hit.rigidbody.AddForce(playerCam.transform.forward * shootStrength);
           
            
            
        }
    }

  
}
