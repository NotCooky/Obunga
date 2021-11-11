using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    public Transform weaponHolder;
    public float grabDistance = 4f;
    GameObject currentWeapon;
    GameObject wp;

    public ProjectileGun ProjectileGun;


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
                    Drop();

                Pickup();
                
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

    void Pickup()
    {
        currentWeapon = wp;
        currentWeapon.transform.position = weaponHolder.position;
        currentWeapon.transform.parent = weaponHolder;
        currentWeapon.transform.localEulerAngles = new Vector3(0, 0, 0);
        currentWeapon.GetComponent<Rigidbody>().isKinematic = true;
        currentWeapon.GetComponent<Collider>().isTrigger = true;
        currentWeapon.GetComponent<ProjectileGun>().enabled = true;
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
