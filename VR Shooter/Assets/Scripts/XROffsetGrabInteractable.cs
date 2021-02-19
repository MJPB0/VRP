using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROffsetGrabInteractable : XRGrabInteractable
{
    [SerializeField] bool isOffsetGrabOn;

    private Vector3 initialAttackLocalPos;
    private Quaternion initialAttachLocalRot;

    // Start is called before the first frame update
    void Start()
    {
        if (isOffsetGrabOn)
        {
            if (!attachTransform)
            {
                GameObject grab = new GameObject("Grab Pivot");
                grab.transform.SetParent(transform, false);
                attachTransform = grab.transform;
            }

            initialAttachLocalRot = attachTransform.localRotation;
            initialAttackLocalPos = attachTransform.localPosition;
        }
    }

    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        if (isOffsetGrabOn)
        {
            if (interactor is XRDirectInteractor)
            {
                attachTransform.position = interactor.transform.position;
                attachTransform.rotation = interactor.transform.rotation;
            }
            else
            {
                attachTransform.localPosition = initialAttackLocalPos;
                attachTransform.localRotation = initialAttachLocalRot;
            }
        }

        base.OnSelectEntered(interactor);
    }
}
