using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Lightsaber : UsableItem
{
    private Animator anim;
    private List<ActionBasedController> controllers;

    [SerializeField] private bool isOn;
    [SerializeField] private int damage;

    AudioSource source;
    [Space]
    [SerializeField] AudioClip saberOn;
    [SerializeField] AudioClip saberOf;
    [SerializeField] AudioClip saberConstant;

    [SerializeField]
    [Tooltip("The blade object")]
    private GameObject _blade = null;

    [SerializeField]
    [Tooltip("The empty game object located at the tip of the blade")]
    private GameObject _tip = null;

    [SerializeField]
    [Tooltip("The empty game object located at the base of the blade")]
    private GameObject _base = null;

    private Vector3 _previousTipPosition;
    private Vector3 _previousBasePosition;
    private Vector3 _triggerEnterTipPosition;
    private Vector3 _triggerEnterBasePosition;
    private Vector3 _triggerExitTipPosition;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        controllers = new List<ActionBasedController>();

        //Set starting position for tip and base
        _previousTipPosition = _tip.transform.position;
        _previousBasePosition = _base.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Sliceable"))
        {
            _triggerEnterTipPosition = _tip.transform.position;
            _triggerEnterBasePosition = _base.transform.position;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!isOn)
            return;

        if (other.gameObject.CompareTag("Sliceable"))
        {
            _triggerExitTipPosition = _tip.transform.position;

            //Create a triangle between the tip and base so that we can get the normal
            Vector3 side1 = _triggerExitTipPosition - _triggerEnterTipPosition;
            Vector3 side2 = _triggerExitTipPosition - _triggerEnterBasePosition;

            //Get the point perpendicular to the triangle above which is the normal
            //https://docs.unity3d.com/Manual/ComputingNormalPerpendicularVector.html
            Vector3 normal = Vector3.Cross(side1, side2).normalized;

            //Transform the normal so that it is aligned with the object we are slicing's transform.
            Vector3 transformedNormal = ((Vector3)(other.gameObject.transform.localToWorldMatrix.transpose * normal)).normalized;

            //Get the enter position relative to the object we're cutting's local transform
            Vector3 transformedStartingPoint = other.gameObject.transform.InverseTransformPoint(_triggerEnterTipPosition);

            Plane plane = new Plane();

            plane.SetNormalAndPosition(
                    transformedNormal,
                    transformedStartingPoint);

            var direction = Vector3.Dot(Vector3.up, transformedNormal);

            Sliceable sliceable = other.gameObject.GetComponent<Sliceable>();
            if (sliceable == null) return;

            //Flip the plane so that we always know which side the positive mesh is on
            if (direction < 0) plane = plane.flipped;

            GameObject[] slices = Slicer.Slice(plane, other.gameObject);

            Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
            Vector3 newNormal = transformedNormal + Vector3.up * sliceable.GetForceAppliedToCut;
            rigidbody.AddForce(newNormal, ForceMode.Impulse);

            rigidbody = slices[0].GetComponent<Rigidbody>();
            newNormal = transformedNormal + Vector3.up * sliceable.GetForceAppliedToCut * -1;
            rigidbody.AddForce(newNormal, ForceMode.Impulse);

            slices[0].transform.parent = other.transform.parent;
            slices[1].transform.parent = other.transform.parent;

            if (sliceable.CutsToDissapear <= 1 && !sliceable.WillDissapear)
            {
                slices[0].tag = "Untagged";
                slices[1].tag = "Untagged";
                slices[0].GetComponent<Sliceable>().enabled = false;
                slices[1].GetComponent<Sliceable>().enabled = false;
            }
            else if (sliceable.CutsToDissapear < 1 && sliceable.WillDissapear)
            {
                Destroy(slices[0], sliceable.GetDissapearTime);
                Destroy(slices[1], sliceable.GetDissapearTime);
            }

            Destroy(other.gameObject);
        }
    }

    public void ToggleLightsaber()
    {
        if (anim.GetBool("isOn"))
        {
            source.clip = null;
            source.loop = false;
            source.PlayOneShot(saberOf);
            anim.SetBool("isOn", false);
            isOn = false;
        }
        else
        {
            source.PlayOneShot(saberOn);
            source.clip = saberConstant;
            source.loop = true;
            source.Play();
            anim.SetBool("isOn", true);
            isOn = true;
        }
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
        ToggleLightsaber();
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

        if (isOn && controllers.Count <= 0)
            ToggleLightsaber();

        controller.activateAction.action.started -= Use;
    }

    private void IsDoubleHolding(bool isDoubleHolding)
    {
        var hands = FindObjectsOfType<Hand>();
        foreach (Hand hand in hands)
            hand.DoubleHolding(isDoubleHolding);
    }
}
