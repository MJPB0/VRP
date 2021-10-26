using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshToMesh : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skinnedMesh;
    [SerializeField] private VisualEffect VFXGraph;
    [SerializeField] private float refreshRate;
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator UpdateVFXGraph()
    {
        while (gameObject.activeSelf)
        {
            Mesh m = new Mesh();
            skinnedMesh.BakeMesh(m);

            Vector3[] vertices = m.vertices;
            Mesh m2 = new Mesh();
            m2.vertices = vertices;

            VFXGraph.SetMesh("Mesh", m2);

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
