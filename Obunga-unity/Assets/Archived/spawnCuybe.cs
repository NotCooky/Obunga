using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnCuybe : MonoBehaviour
{
    public GameObject Bread;
    public GameObject Ham;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            Instantiate(Bread, new Vector3(0, 0, 0), Quaternion.identity);

            Debug.Log("W has been pressed");
        }

        if (Input.GetKeyDown("o"))
        {
            Instantiate(Ham, new Vector3(0, 0, 0), Quaternion.identity);

            Debug.Log("a has been pressed");





        }
    }
}
