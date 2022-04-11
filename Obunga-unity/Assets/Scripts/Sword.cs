using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            StartCoroutine("Slice");
        }
    }

    IEnumerator Slice()
    {
        animator.SetBool("Slice", true);
        yield return null;
        animator.SetBool("Slice", false);
    }
}
