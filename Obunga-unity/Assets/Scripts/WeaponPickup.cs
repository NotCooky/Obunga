using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Transform weaponHolder;
    public Transform playerCamera;
    GameObject pickedUpWeapon;

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

        if(Physics.Raycast(playerCamera.position, transform.forward, out hit, 3f))
        {
            if(hit.transform.gameObject.tag == "Weapon")
            {
                pickedUpWeapon = hit.transform.gameObject;

                pickedUpWeapon.transform.parent = weaponHolder.transform;
                pickedUpWeapon.transform.position = Vector3.Lerp(pickedUpWeapon.transform.position, weaponHolder.transform.position, 0.1f);
                pickedUpWeapon.transform.rotation = Quaternion.Slerp(pickedUpWeapon.transform.rotation, weaponHolder.transform.rotation, 0.1f);

                pickedUpWeapon.GetComponent<Rigidbody>().isKinematic = true;
                pickedUpWeapon.GetComponent<Animator>().enabled = true;
                pickedUpWeapon.GetComponent<Gun>().enabled = true;
                pickedUpWeapon.GetComponent<BoxCollider>().enabled = false;
                print("picked up");
            }
        }
    }
}
