using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float despawnTimer = 0.002f;
    void Start()
    {
        StartCoroutine("DeleteLaser");
    }
    IEnumerator DeleteLaser()
    {
        yield return new WaitForSeconds(despawnTimer);
        Destroy(gameObject);
        yield return new WaitForSeconds(despawnTimer);
    }

}
