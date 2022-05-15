using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Gun : MonoBehaviour
{
    public GunData gunData;
    public Transform camHolder;
    public Animator animator;
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip[] shootSFX;

    float timeSinceLastShot;

    RaycastHit hit;
    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60);

    private void Start()
    {
        PlayerShoot.shootInput += Shoot;
        PlayerShoot.reloadInput += StartReload;
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    private void OnDisable() => gunData.reloading = false;

    public void StartReload()
    {
        if(!gunData.reloading && this.gameObject.activeSelf)
        {
            StartCoroutine(Reload());
        }
    }
    private IEnumerator Reload()
    {
        gunData.reloading = true;
        animator.SetTrigger("Reload");
        yield return new WaitForSeconds(gunData.reloadTime);
        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
    }

    public void Shoot()
    {
        if(gunData.currentAmmo > 0 && CanShoot())
        {
            if(Physics.Raycast(camHolder.position, camHolder.forward, out hit, gunData.maxDistance))
            {
                IDamagable damageable = hit.transform.GetComponent<IDamagable>();
                damageable?.TakeDamage(gunData.damage);
            }

            CameraShaker.Instance.ShakeOnce(0.4f, 2f, 0f, 0f);
            audioSource.PlayOneShot(shootSFX[Random.Range(0, shootSFX.Length - 1)]);
            muzzleFlash.Play();
            gunData.currentAmmo--;
            timeSinceLastShot = 0;
            OnGunShot();
        }
    }
    private void OnGunShot()
    {
        animator.SetTrigger("Shoot");
    }
}
