using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Shotgun : MonoBehaviour
{
    public Camera playerCam;
    public Rigidbody playerRb;
    public ParticleSystem muzzleFlash;
    public Animator gunAnimator;
    public float magazineSize;
    public AudioClip[] gunSFX;
    public AudioClip[] reloadSFX;
    LayerMask PlayerLayer;
    AudioSource gunAudioSource;

    bool canShoot = true;
    float bulletsLeftInMag;
    float range = 100f;
    float impactForce = 500f;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = GameObject.Find("Camera").GetComponent<Camera>();
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody>();
        PlayerLayer = LayerMask.GetMask("Player");
        gunAudioSource = GetComponent<AudioSource>();
        bulletsLeftInMag = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if(Input.GetKeyDown(KeyCode.R) && bulletsLeftInMag != magazineSize)
        {
            StartCoroutine("Reload");
        }
    }

    void Shoot()
    {
        if (canShoot && bulletsLeftInMag > 0)
        {
            bulletsLeftInMag--;
            muzzleFlash.Play();
            gunAnimator.SetTrigger("Shoot");
            gunAudioSource.PlayOneShot(gunSFX[Random.Range(0, gunSFX.Length - 1)]);
            CameraShaker.Instance.ShakeOnce(30f, 0.3f, 0, 0.5f);
            playerRb.AddForce(-playerCam.transform.forward * 500, ForceMode.Impulse);


            RaycastHit hit;

            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range, ~PlayerLayer))
            {
                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }
            }
        }  
    }

    IEnumerator Reload()
    {
        canShoot = false;
        bulletsLeftInMag = magazineSize;
        gunAnimator.SetTrigger("Reload");
        gunAudioSource.PlayOneShot(reloadSFX[Random.Range(0, reloadSFX.Length - 1)]);
        yield return new WaitForSeconds(0.5f);
        canShoot = true;
    }
}
