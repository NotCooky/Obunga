using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTilt : MonoBehaviour
{

    public float leanAngle = 5f;

    float curAngle;
    float targetAngle;
    float angle;

    float maxRot = -45.0f;
    float rate = 2.0f;

    void Update()
    {
        LeanCamera(Input.GetAxis("Horizontal"));
    }

    public void LeanCamera(float axis)
    {
        curAngle = transform.localEulerAngles.z;
        targetAngle = leanAngle - axis;

        if (axis == 0.0f) targetAngle = 0.0f;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, axis * maxRot), Time.deltaTime * rate);

    }
}
