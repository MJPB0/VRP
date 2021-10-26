using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class UsableItem : MonoBehaviour
{
    public abstract void PickedUp(ActionBasedController controller);

    public abstract void Use(InputAction.CallbackContext context);

    public abstract void PutDown(ActionBasedController controller);
}
