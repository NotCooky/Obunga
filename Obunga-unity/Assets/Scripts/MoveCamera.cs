using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform cameraPos;
    public Vector3 vaultOffset;
    void Update() 
    {
        transform.position = cameraPos.position + vaultOffset;
        vaultOffset = Vector3.Slerp(vaultOffset, Vector3.zero, Time.deltaTime * 3f);
    }
}
