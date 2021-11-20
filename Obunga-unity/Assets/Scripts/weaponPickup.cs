using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    public Transform weaponHolder;
    public float grabDistance = 4f;
    GameObject currentWeapon;

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

        if (isGrabbable && Input.GetKey(KeyCode.E))
        {
            Pickup();
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
                currentWeapon = hit.transform.gameObject;
            }
        }
        else
        {
            isGrabbable = false;
        }
    }

    void Pickup()
    {

        currentWeapon.transform.position = weaponHolder.position;
        currentWeapon.transform.parent = weaponHolder;
        currentWeapon.transform.rotation = weaponHolder.rotation;
        currentWeapon.GetComponent<Rigidbody>().isKinematic = true;
        currentWeapon.GetComponent<Collider>().isTrigger = true;
        currentWeapon.GetComponent<ProjectileGun>().enabled = true;
    }
}
