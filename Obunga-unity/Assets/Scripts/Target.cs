using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour, IDamagable
{
    float health = 100f;
    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0) Destroy(gameObject);
    }
}
