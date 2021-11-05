using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway2 : MonoBehaviour
{
    public float amount;
    public float maxAmount;
    public float smoothAmount;

    private Vector3 initalPosition;

    private float movementX;
    private float movementY;

    // Start is called before the first frame update
    void Start()
    {
        initalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        movementX = -Input.GetAxisRaw("Mouse X");
        movementY = -Input.GetAxisRaw("Mouse Y");

        float moveX = Mathf.Clamp(movementX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(movementY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        //transfor
    }
}
