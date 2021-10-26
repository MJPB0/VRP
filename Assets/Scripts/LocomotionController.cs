using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class LocomotionController : MonoBehaviour
{
    [SerializeField] float teleportationToggleOffset = .2f;
    public XRController teleportRay;
    public InputHelpers.Button teleportActivationButton;

    [SerializeField] float additionalHeight =  .2f;
    private CharacterController character;
    private XRRig rig;

    private void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XRRig>();
    }

    void Update()
    {
        if (teleportRay)
        {
            teleportRay.gameObject.SetActive(CheckIfActivated(teleportRay));
        } 
    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();
    }

    public bool CheckIfActivated (XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportActivationButton, out bool isActivated, teleportationToggleOffset);
        return isActivated;
    }

    void CapsuleFollowHeadset()
    {
        character.height = rig.cameraInRigSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height/2 + character.skinWidth, capsuleCenter.z);
    }
}
