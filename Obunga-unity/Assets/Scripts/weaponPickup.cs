using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    public Transform weaponHolder;
    public float grabDistance = 4f;
    GameObject currentWeapon;
    GameObject wp;

    float pickupDuration = 0.5f;


    public float dropForwardForce;
    public float dropUpwardForce;
    public Transform playerCamera;

    private ProjectileGun ProjectileGun;
   

    bool isGrabbable;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckWeapons();

        if (isGrabbable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentWeapon != null)
                {
                    Drop();
                }           
                StartCoroutine("Pickup");
            }
        }

        if (currentWeapon != null && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    void CheckWeapons()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, grabDistance))
        {
            if (hit.transform.tag == "Grabbable")
            {
                Debug.Log("Grabbable");
                isGrabbable = true;
                wp = hit.transform.gameObject;
            }
        }
        else
        {
            isGrabbable = false;
        }
    }

    IEnumerator Pickup()
    {
        currentWeapon = wp;

        float t = 0.0f;
        while (t < pickupDuration)
        {
            t += Time.deltaTime;
            float normalizedTime = t / pickupDuration;

            currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, weaponHolder.position, normalizedTime);
            currentWeapon.transform.rotation = Quaternion.Slerp(currentWeapon.transform.rotation, weaponHolder.rotation, normalizedTime);
            yield return null;
        }
        currentWeapon.transform.parent = weaponHolder; 
        currentWeapon.GetComponent<Rigidbody>().isKinematic = true;
        currentWeapon.GetComponent<Collider>().isTrigger = true;
        currentWeapon.GetComponent<ProjectileGun>().enabled = true;
        yield return new WaitForSeconds(2);
    }

    void Drop()
    {
        currentWeapon.transform.parent = null;
        currentWeapon.GetComponent<Rigidbody>().isKinematic = false;
        currentWeapon.GetComponent<Collider>().isTrigger = false;
        currentWeapon.GetComponent<ProjectileGun>().enabled = false;
        currentWeapon = null;
    }
}
