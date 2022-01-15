using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Gun gunScript;
    public Transform weaponHolder;
    public Transform playerCamera;
    GameObject pickedUpWeapon;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        RaycastHit hit;

        if(Physics.Raycast(playerCamera.position, transform.forward, out hit, 1f))
        {
            if(hit.transform.gameObject.tag == "Weapon")
            {
                pickedUpWeapon = hit.transform.gameObject;


                pickedUpWeapon.transform.position = Vector3.Lerp(pickedUpWeapon.transform.position, weaponHolder.transform.position, 1f);
                pickedUpWeapon.transform.rotation = Quaternion.Slerp(pickedUpWeapon.transform.rotation, weaponHolder.transform.rotation, 1f);
            }
        }
    }
}
