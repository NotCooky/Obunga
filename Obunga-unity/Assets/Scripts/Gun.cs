
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
<<<<<<< HEAD
    public float range = 100f;
    public float fireRate;
    public float shootStrength;
    public ParticleSystem muzzleFlash;
    private Animator animator;
=======
    float damage = 10f;
    float range = 100f;
>>>>>>> parent of 47b7cc1 (what.)
    
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 0.5f;
    private bool isReloading = false;

    public Camera playerCam;

<<<<<<< HEAD
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
=======
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
>>>>>>> parent of 47b7cc1 (what.)
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
<<<<<<< HEAD
            hit.rigidbody.AddForce(playerCam.transform.forward * shootStrength);
=======
            Debug.Log(hit.transform.name);
>>>>>>> parent of 47b7cc1 (what.)
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
