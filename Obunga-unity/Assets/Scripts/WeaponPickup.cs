using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public Transform weaponHolder;
    public Transform camHolder;
    GameObject pickedUpWeapon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(camHolder.position, camHolder.forward * 3f, Color.red);
        if (Input.GetKeyDown(KeyCode.F))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        RaycastHit hit;
        if(Physics.Raycast(camHolder.position, camHolder.forward, out hit, 3f))
        {
            if(hit.transform.tag == "Weapon")
            {
                pickedUpWeapon = hit.transform.gameObject;
                pickedUpWeapon.transform.parent = weaponHolder;
                pickedUpWeapon.transform.position = weaponHolder.transform.position;
                print("help");
            }
        }
    }
}
