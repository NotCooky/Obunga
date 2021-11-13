using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponPickup : MonoBehaviour
{
    public Transform weaponHolder;
    public float grabDistance = 4f;
    GameObject currentWeapon;
    //Transform currentWeapon;
    GameObject wp;


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
         currentWeapon.transform.localEulerAngles = new Vector3(0, 0, 0);
        //currentWeapon.transform.position = Vector3.Lerp(currentWeapon.position, weaponHolder.position, Time.deltaTime);
        //currentWeapon.transform.rotation = Quaternion.Lerp(currentWeapon.rotation, weaponHolder.rotation, Time.deltaTime * lerpSpeed); 
        currentWeapon.transform.parent = weaponHolder; 
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

        //currentWeapon.GetComponent<Rigidbody>().AddForce(playerCamera.forward * dropForwardForce, ForceMode.Impulse);
        //currentWeapon.GetComponent<Rigidbody>().AddForce(playerCamera.up * dropUpwardForce, ForceMode.Impulse);
        //float random = Random.Range(-1f, 1f);
        //currentWeapon.GetComponent<Rigidbody>().AddTorque(new Vector3(random, random, random) * 10);
    }
}
