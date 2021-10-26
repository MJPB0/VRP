using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;

public class Sword : UsableItem
{
    private List<ActionBasedController> controllers;

    [SerializeField] private float damage;

    [Space]
    [SerializeField] private float activatedTime;
    [SerializeField] private float currentActivatedTime;
    [SerializeField] private bool canActivate = true;
    [SerializeField] private bool activated = false;
    [SerializeField] private float activateCooldown;
    [SerializeField] private float currentActivateCooldown;

    [Space]
    [SerializeField] VisualEffect swordVFX;

    private void Start()
    {
        controllers = new List<ActionBasedController>();
        swordVFX.enabled = false;
    }

    private void Update()
    {
        SwordActivation();
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

    public override void Use(InputAction.CallbackContext context)
    {
        ActivateSword();
    }

    private void IsDoubleHolding(bool isDoubleHolding)
    {
        var hands = FindObjectsOfType<Hand>();
        foreach (Hand hand in hands)
            hand.DoubleHolding(isDoubleHolding);
    }

    private void ActivateSword()
    {
        if (!canActivate && currentActivateCooldown > 0f)
            return;
        currentActivateCooldown = activateCooldown;
        activated = true;
        canActivate = false;
        swordVFX.enabled = true;
    }

    private void DeactivateSword()
    {
        canActivate = false;
        activated = false;
        currentActivatedTime = 0f;
        currentActivateCooldown = activateCooldown;
        swordVFX.enabled = false;
    }

    private void SwordActivation()
    {
        if (activated && currentActivatedTime < activatedTime)
            currentActivatedTime += Time.deltaTime;
        else if (activated)
            DeactivateSword();

        if (!activated && currentActivateCooldown > 0f)
            currentActivateCooldown -= Time.deltaTime;
        else if (!activated)
        {
            currentActivateCooldown = 0f;
            canActivate = true;
        }
    }
}
