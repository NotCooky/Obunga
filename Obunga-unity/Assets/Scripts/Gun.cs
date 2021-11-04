
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float range = 100f;
    public float fireRate;
    public float shootStrength;
    public ParticleSystem muzzleFlash;
    private Animator animator;
    
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 0.5f;
    private bool isReloading = false;

    public Camera playerCam;

    private float nextTimeToFire;

    void Start()
    {
       animator = GetComponent<Animator>();
       currentAmmo = maxAmmo;
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
            muzzleFlash.Play();
            nextTimeToFire= Time.time + 1f / fireRate;
            Shoot();
            
            
        }
    }

    void Shoot()
    {
        currentAmmo--;

        animator.SetBool("shooting", true);

        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {
            hit.rigidbody.AddForce(playerCam.transform.forward * shootStrength);
        }

        animator.SetBool("shooting", false);
    }

    IEnumerator Reload ()
    {
        isReloading = true;
        Debug.Log("Reloading.");

        animator.SetBool("Reloading", true);

        yield return new WaitForSeconds(reloadTime);

        animator.SetBool("Reloading", false);

        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
