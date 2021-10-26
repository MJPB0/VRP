using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Range(0,2)]
    [SerializeField] float damageMultiplier;
    private float damage;

    public void SetDamage(int gunDamage)
    {
        damage = gunDamage * damageMultiplier;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
