using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Gun : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bullet;
    [SerializeField] float initialSpeed;

    AudioSource source;
    [SerializeField] AudioClip gunshotSound;
    [SerializeField] VisualEffect muzzleFlash;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Fire()
    {
        GameObject firedBullet = Instantiate(bullet, firePoint.position, firePoint.rotation);
        Vector3 force = firePoint.forward * initialSpeed;
        firedBullet.GetComponent<Rigidbody>().AddForce(force);

        source.PlayOneShot(gunshotSound);
        muzzleFlash.Play();
    }

}
