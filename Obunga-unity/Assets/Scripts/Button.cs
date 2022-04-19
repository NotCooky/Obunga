using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : Interactable
{
    public UnityEvent onButtonPress;
    public AnimationClip buttonPress;
    Animator buttonAnim;


    private void Start()
    {
        buttonAnim = GetComponent<Animator>();
    }
    // Update is called once per frame
    public override void Interact()
    {
        buttonAnim.SetTrigger("Press");
        onButtonPress.Invoke();
    }
}
