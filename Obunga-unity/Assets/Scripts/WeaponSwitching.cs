using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [Header("Refrences")]
    public Transform[] weapons;

    [Header("Keys")]
    public KeyCode[] keys;

    [Header("Setting")]
    public float switchTime;

    int selectedWeapon;
    float timeSinceLastSwitch;

    private void Start()
    {
        SetWeapons();
        Select(selectedWeapon);

        timeSinceLastSwitch = 0f;
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < keys.Length; i++)
        {
            if(Input.GetKeyDown(keys[i]) && timeSinceLastSwitch > switchTime)
            {
                selectedWeapon = i;
            }
        }

        if(previousSelectedWeapon != selectedWeapon) Select(selectedWeapon);

        timeSinceLastSwitch += Time.deltaTime;
    }

    private void SetWeapons()
    {
        weapons = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i);
        }

        if(keys == null) keys = new KeyCode[weapons.Length];
    }

    private void Select(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }

        timeSinceLastSwitch = 0f;
        OnWeaponSelected();
    }

    private void OnWeaponSelected()
    {
        print("Selected new weapon...");
    }





}
