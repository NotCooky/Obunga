using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Gun : MonoBehaviour
{
    public Camera playerCam;
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
        PlayerLayer = LayerMask.GetMask("Player");
        gunAudioSource = GetComponent<AudioSource>();
        playerCam = GetComponentInParent<Camera>();
        bulletsLeftInMag = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if(bulletsLeftInMag == 0)
        {
            StartCoroutine("Reload");
        }
    }

    void Shoot()
    {
        if (canShoot)
        {
            bulletsLeftInMag--;
            muzzleFlash.Play();
            gunAnimator.SetTrigger("Shoot");
            gunAudioSource.PlayOneShot(gunSFX[Random.Range(0, gunSFX.Length - 1)]);
            CameraShaker.Instance.ShakeOnce(2f, 5f, 0, 0.5f);


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
