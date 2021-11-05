using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway2 : MonoBehaviour
{
    //position
    public float amount;
    public float maxAmount;
    public float smoothAmount;

    //rotation
    public float rotationAmount;
    public float maxRotationAmount;
    public float smoothRotation;

    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;


    private Vector3 initalPosition;
    private Quaternion initialRotation;

    private float InputX;
    private float InputY;

    // Start is called before the first frame update
    void Start()
    {
        initalPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateSway();
        MoveSway();
        TiltSway();

    }

    void CalculateSway()
    {
        InputX = -Input.GetAxisRaw("Mouse X");
        InputY = -Input.GetAxisRaw("Mouse Y");
    }

    void MoveSway()
    {
        
        float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(InputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initalPosition, Time.deltaTime * smoothAmount);
    }

    void TiltSway()
    {
        float tiltY = Mathf.Clamp(InputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
        float tiltX = Mathf.Clamp(InputY * rotationAmount, -maxRotationAmount, maxRotationAmount);

        Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY : 0, rotationZ ? tiltY : 0f));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, Time.deltaTime * smoothRotation);
    }
}
