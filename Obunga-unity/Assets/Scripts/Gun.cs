using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GunData gunData;
    public Transform cam;

    float timeSinceLastShot;

    RaycastHit hit;
    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f / (gunData.fireRate / 60);

    private void Start()
    {
        PlayerShoot.shootInput += Shoot;
        PlayerShoot.reloadInput += StartReload;
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
        yield return new WaitForSeconds(gunData.reloadTime);
        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
    }

    public void Shoot()
    {
        if(gunData.currentAmmo > 0 && CanShoot())
        {
            if(Physics.Raycast(cam.position, cam.forward, out hit, gunData.maxDistance))
            {
                IDamagable damageable = hit.transform.GetComponent<IDamagable>();
                damageable?.TakeDamage(gunData.damage);
            }

            gunData.currentAmmo--;
            timeSinceLastShot = 0;
            OnGunShot();
        }
    }
    private void OnGunShot()
    {

    }
}
