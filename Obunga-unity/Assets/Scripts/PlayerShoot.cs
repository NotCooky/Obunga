using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public static Action shootInput;
    public static Action reloadInput;

    public KeyCode reloadKey;

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            shootInput?.Invoke();
        }

        if(Input.GetKeyDown(reloadKey))
        {
            reloadInput?.Invoke();
        }
    }
}
