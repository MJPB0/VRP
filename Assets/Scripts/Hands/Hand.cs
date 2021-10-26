using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Animator))]
public class Hand : MonoBehaviour
{
    [SerializeField] private float breakForce = 50f;
    [SerializeField] private float breakTorque = 10f;

    [Space]
    [SerializeField] private ActionBasedController controller;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float rotateSpeed = 100f;

    [Space]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    [Space]
    [SerializeField] private Transform palm;
    [SerializeField] private float reachDistance = .1f;
    [SerializeField] private float joinDistance = .05f;
    [SerializeField] private LayerMask grabbableLayer;

    [Space]
    [SerializeField] private float animationSpeed = 20f;

    private const string _gripAnimatorParam = "Grip";
    private const string _triggerAnimatorParam = "Trigger";
    private const string _usableTag = "Usable";

    private Animator animator;
    private float gripCurrent;
    private float triggerCurrent;

    private Transform followTarget;
    private Rigidbody body;

    [Space]
    [SerializeField] private bool isGrabbing;
    [SerializeField] private GameObject heldObject;
    private Transform grabPoint;
    private FixedJoint joint1;
    private FixedJoint joint2;

    private float baseBreakForce = 1500f;
    private float baseBreakTorque = 1250f;

    public bool IsHoldingItem() { return isGrabbing && heldObject; }
    public GameObject HeldObject() { return heldObject; }
    public ActionBasedController GetActionBasedController { get { return controller; } }

    public void DoubleHolding(bool isDoubleHolding)
    {
        if (isDoubleHolding)
        {
            breakForce = float.PositiveInfinity;
            breakTorque = float.PositiveInfinity;
        }
        else
        {
            breakForce = baseBreakForce;
            breakTorque = baseBreakTorque;
        }
    }

    void Start()
    {
        followTarget = controller.gameObject.transform;
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        PresetRigidbody();

        controller.selectAction.action.started += Grab;
        controller.selectAction.action.canceled += Release;

        body.position = followTarget.position;
        body.rotation = followTarget.rotation;
    }

    void Update()
    {
        AnimateHand();
        PhysicsMove();
    }

    private void PresetRigidbody()
    {
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.mass = 20f;
        body.maxAngularVelocity = 20f;
    }

    private void PhysicsMove()
    {
        var positionWithOffset = followTarget.TransformPoint(positionOffset);
        var distance = Vector3.Distance(positionWithOffset, transform.position);
        body.velocity = (positionWithOffset - transform.position).normalized * followSpeed * distance;

        var rotationWithOffset = followTarget.rotation * Quaternion.Euler(rotationOffset);
        var q = rotationWithOffset * Quaternion.Inverse(body.rotation);
        q.ToAngleAxis(out float angle, out Vector3 axis);
        body.angularVelocity = angle * axis * Mathf.Deg2Rad * rotateSpeed;
    }

    private void Grab(InputAction.CallbackContext obj)
    {
        if (isGrabbing || heldObject) return;

        Collider[] colliders = Physics.OverlapSphere(palm.position, reachDistance, grabbableLayer);
        if (colliders.Length < 1) return;

        var objectToGrab = colliders[0].transform.gameObject;
        var objectBody = objectToGrab.GetComponent<Rigidbody>();
        if (objectBody != null) 
            heldObject = objectBody.gameObject;
        else
        {
            objectBody = objectToGrab.GetComponentInParent<Rigidbody>();
            if (objectBody != null)
                heldObject = objectBody.gameObject;
            else
                return;
        }

        StartCoroutine(GrabObject(colliders[0], objectBody));
    }

    private IEnumerator GrabObject(Collider collider, Rigidbody targetBody)
    {
        isGrabbing = true;
        grabPoint = new GameObject().transform;
        grabPoint.position = collider.ClosestPoint(palm.position);
        grabPoint.parent = heldObject.transform;

        followTarget = grabPoint;

        while (grabPoint != null && Vector3.Distance(grabPoint.position, palm.position) > joinDistance && isGrabbing)
        {
            yield return new WaitForEndOfFrame();
        }

        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        targetBody.velocity = Vector3.zero;
        targetBody.angularVelocity = Vector3.zero;

        targetBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        targetBody.interpolation = RigidbodyInterpolation.Interpolate;

        joint1 = gameObject.AddComponent<FixedJoint>();
        joint1.connectedBody = targetBody;
        joint1.breakForce = breakForce;
        joint1.breakTorque = breakTorque;

        joint1.connectedMassScale = 1;
        joint1.massScale = 1;
        joint1.enableCollision = false;
        joint1.enablePreprocessing = false;

        joint2 = heldObject.AddComponent<FixedJoint>();
        joint2.connectedBody = body;
        joint2.breakForce = breakForce;
        joint2.breakTorque = breakTorque;
             
        joint2.connectedMassScale = 1;
        joint2.massScale = 1;
        joint2.enableCollision = false;
        joint2.enablePreprocessing = false;

        followTarget = controller.gameObject.transform;

        if (heldObject && heldObject.CompareTag(_usableTag))
            heldObject.GetComponent<UsableItem>().PickedUp(controller);
    }

    private void Release(InputAction.CallbackContext obj)
    {
        if (joint1 != null)
            Destroy(joint1);
        if (joint2 != null)
            Destroy(joint2);
        if (grabPoint != null)
            Destroy(grabPoint.gameObject);

        if (heldObject && heldObject.CompareTag(_usableTag))
            heldObject.GetComponent<UsableItem>().PutDown(controller);

        if (heldObject != null)
        {
            var targetBody = heldObject.GetComponent<Rigidbody>();
            targetBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            targetBody.interpolation = RigidbodyInterpolation.None;
            heldObject = null;
        }

        isGrabbing = false;
        followTarget = controller.gameObject.transform;
    }

    private void OnJointBreak(float breakForce)
    {
        Release(new InputAction.CallbackContext());
    }

    private void AnimateHand()
    {
        if (isGrabbing && heldObject) return;

        float gripTarget = controller.selectAction.action.ReadValue<float>();
        float triggerTarget = controller.activateAction.action.ReadValue<float>();

        if (gripTarget != gripCurrent)
        {
            gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime * animationSpeed);
            animator.SetFloat(_gripAnimatorParam, gripCurrent);
        }
        if (triggerTarget != triggerCurrent)
        {
            triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, Time.deltaTime * animationSpeed);
            animator.SetFloat(_triggerAnimatorParam, triggerCurrent);
        }
    }
}
