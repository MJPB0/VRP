using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;

public class Gun : UsableItem
{
    public int gunDamage;
    private List<ActionBasedController> controllers;

    [Space]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float initialSpeed;

    AudioSource source;
    [SerializeField] private AudioClip gunshotSound;
    [SerializeField] private VisualEffect muzzleFlash;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        controllers = new List<ActionBasedController>();

        bullet.GetComponent<Bullet>().SetDamage(gunDamage);
    }

    public void Fire()
    {
        GameObject firedBullet = Instantiate(bullet, firePoint.position, firePoint.rotation);
        Vector3 force = firePoint.forward * initialSpeed;
        firedBullet.GetComponent<Rigidbody>().AddForce(force);

        source.PlayOneShot(gunshotSound);
        muzzleFlash.Play();
    }

    public override void PickedUp(ActionBasedController controller)
    {
        if (controller == null) return;
        if (!controllers.Contains(controller))
            controllers.Add(controller);

        if (controllers.Count == 2)
            IsDoubleHolding(true);
        else
            IsDoubleHolding(false);

        controller.activateAction.action.started += Use;
    }

    public override void Use(InputAction.CallbackContext context)
    {
        Fire();
    }

    public override void PutDown(ActionBasedController controller)
    {
        if (controller == null) return;
        if (controllers.Contains(controller))
            controllers.Remove(controller);

        if (controllers.Count == 2)
            IsDoubleHolding(true);
        else
            IsDoubleHolding(false);

        controller.activateAction.action.started -= Use;
    }

    private void IsDoubleHolding(bool isDoubleHolding)
    {
        var hands = FindObjectsOfType<Hand>();
        foreach (Hand hand in hands)
            hand.DoubleHolding(isDoubleHolding);
    }
}
