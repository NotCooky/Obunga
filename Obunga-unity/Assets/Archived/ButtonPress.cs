using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{

    public GameObject thing;
    public Transform spawnPoint;

    // Make sure to set this method as public!
    public void ButtonPressed()
    {
        // Your instantiate code here
        Instantiate(thing, spawnPoint);
    }
}
