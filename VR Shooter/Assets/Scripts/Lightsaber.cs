using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Lightsaber : MonoBehaviour
{
    Animator anim;

    AudioSource source;
    [SerializeField] AudioClip saberOn;
    [SerializeField] AudioClip saberOf;
    [SerializeField] AudioClip saberConstant;

    [SerializeField] bool isOn;

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

            //Flip the plane so that we always know which side the positive mesh is on
            if (direction < 0)
            {
                plane = plane.flipped;
            }

            GameObject[] slices = Slicer.Slice(plane, other.gameObject);

            Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
            Vector3 newNormal = transformedNormal + Vector3.up * other.gameObject.GetComponent<Sliceable>().getForceAppliedToCut();
            rigidbody.AddForce(newNormal, ForceMode.Impulse);

            slices[0].transform.parent.gameObject.layer = LayerMask.NameToLayer("Grab");
            if (other.gameObject.GetComponent<Sliceable>().WillDissapear())
                Destroy(slices[0].transform.parent.gameObject, other.gameObject.GetComponent<Sliceable>().getDissapearTime());
            if (other.gameObject.GetComponent<Sliceable>().CutsToDissapear <= 1)
            {
                slices[0].GetComponent<Sliceable>().enabled = false;
                slices[1].GetComponent<Sliceable>().enabled = false;
                slices[0].tag = "Untagged";
                slices[1].tag = "Untagged";
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
}
