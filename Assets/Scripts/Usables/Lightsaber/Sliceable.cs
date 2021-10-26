using UnityEngine;

public class Sliceable : MonoBehaviour
{
    [SerializeField]
    private bool _isSolid = true;

    [SerializeField]
    private bool _reverseWindTriangles = false;

    [SerializeField]
    private bool _useGravity = false;

    [SerializeField]
    private bool _shareVertices = false;

    [SerializeField]
    private bool _smoothVertices = false;

    [SerializeField]
    [Tooltip("The amount of force applied to each side of a slice")]
    private float _forceAppliedToCut = 3f;

    [SerializeField]
    [Tooltip("Is the object going to dissapear after being cut")]
    private bool _willDissapear;

    [SerializeField]
    [Tooltip("Time after which sliced objects dissapear")]
    private float _dissapearTime;

    [SerializeField]
    [Tooltip("Will it dissapear after one cut")]
    [Range(1,10)]
    private int _cutsToDissapear;

    public float GetForceAppliedToCut { get { return _forceAppliedToCut; }  }
    public float GetDissapearTime { get { return _dissapearTime; }  }
    public bool WillDissapear { get { return _willDissapear; } }
    public int CutsToDissapear { get { return _cutsToDissapear; } set { _cutsToDissapear = value; } }

    public bool IsSolid
    {
        get
        {
            return _isSolid;
        }
        set
        {
            _isSolid = value;
        }
    }

    public bool ReverseWireTriangles
    {
        get
        {
            return _reverseWindTriangles;
        }
        set
        {
            _reverseWindTriangles = value;
        }
    }

    public bool UseGravity
    {
        get
        {
            return _useGravity;
        }
        set
        {
            _useGravity = value;
        }
    }

    public bool ShareVertices
    {
        get
        {
            return _shareVertices;
        }
        set
        {
            _shareVertices = value;
        }
    }

    public bool SmoothVertices
    {
        get
        {
            return _smoothVertices;
        }
        set
        {
            _smoothVertices = value;
        }
    }

}