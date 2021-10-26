using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float Health;

    private Action Death;

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void GetHit(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            Die();
    }

    private void Die()
    {
        Death?.Invoke();
        Destroy(gameObject);
    }
}
