using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Gun : MonoBehaviour
{
    float range = 100f;
    float impactForce = 500f;

    public Camera playerCam;
    public ParticleSystem muzzleFlash;
   // public GameObject impactEffect;
    public Animator gunAnimator;
    public AudioClip[] gunSFX;
    LayerMask PlayerLayer;
    AudioSource gunAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        PlayerLayer = LayerMask.GetMask("Player");
        gunAudioSource = GetComponent<AudioSource>();
        playerCam = GetComponentInParent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        gunAnimator.SetTrigger("Shoot");
        gunAudioSource.PlayOneShot(gunSFX[Random.Range(0, gunSFX.Length - 1)]);
        CameraShaker.Instance.ShakeOnce(2f, 5f, 0, 0.5f);
        

        RaycastHit hit;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range, ~PlayerLayer))
        {
           // GameObject bulletEffect = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
           // Destroy(bulletEffect, 2f);
           if (hit.rigidbody != null)
           {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
           }
        }
    }
}
