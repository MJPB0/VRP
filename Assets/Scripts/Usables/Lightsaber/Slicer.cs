using System;
using UnityEngine;

namespace Assets.Scripts
{
    class Slicer : MonoBehaviour
    {
        private const string _grabbableLayer = "Grabbable";

        /// <summary>
        /// Slice the object by the plane 
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="objectToCut"></param>
        /// <returns></returns>
        public static GameObject[] Slice(Plane plane, GameObject objectToCut)
        {
            //Get the current mesh and its verts and tris
            Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
            Sliceable sliceable = objectToCut.GetComponent<Sliceable>();

            if (sliceable == null) throw new NotSupportedException("Cannot slice non sliceable object, add the sliceable script to the object or inherit from sliceable to support slicing");
            
            //Create left and right slice of hollow object
            SlicesMetadata slicesMeta = new SlicesMetadata(plane, mesh, sliceable.IsSolid, sliceable.ReverseWireTriangles, sliceable.ShareVertices, sliceable.SmoothVertices);

            GameObject positiveObject = CreateMeshGameObject(objectToCut);
            positiveObject.name = string.Format("{0}_positive", objectToCut.name);

            GameObject negativeObject = CreateMeshGameObject(objectToCut);
            negativeObject.name = string.Format("{0}_negative", objectToCut.name);

            var positiveSideMeshData = slicesMeta.PositiveSideMesh;
            var negativeSideMeshData = slicesMeta.NegativeSideMesh;

            positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
            negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

            PhysicMaterial physicMaterial = objectToCut.GetComponent<Collider>().material;

            SetupCollidersAndRigidBodys(ref positiveObject, positiveSideMeshData, sliceable.UseGravity, physicMaterial);
            SetupCollidersAndRigidBodys(ref negativeObject, negativeSideMeshData, sliceable.UseGravity, physicMaterial);

            Sliceable positiveSliceable = positiveObject.GetComponent<Sliceable>();
            Sliceable negativeSliceable = negativeObject.GetComponent<Sliceable>();
            Sliceable objectToCutSliceable = objectToCut.GetComponent<Sliceable>();

            positiveSliceable.CutsToDissapear = objectToCutSliceable.CutsToDissapear - 1;
            negativeSliceable.CutsToDissapear = objectToCutSliceable.CutsToDissapear - 1;

            if (objectToCut.layer == LayerMask.NameToLayer(_grabbableLayer))
            {
                positiveObject.layer = LayerMask.NameToLayer(_grabbableLayer);
                negativeObject.layer = LayerMask.NameToLayer(_grabbableLayer);
            }

            return new GameObject[] { positiveObject, negativeObject };
        }

        /// <summary>
        /// Creates the default mesh game object.
        /// </summary>
        /// <param name="originalObject">The original object.</param>
        /// <returns></returns>
        private static GameObject CreateMeshGameObject(GameObject originalObject)
        {
            var originalMaterial = originalObject.GetComponent<MeshRenderer>().materials;

            GameObject meshGameObject = new GameObject();
            Sliceable originalSliceable = originalObject.GetComponent<Sliceable>();

            meshGameObject.AddComponent<MeshFilter>();
            meshGameObject.AddComponent<MeshRenderer>();
            Sliceable sliceable = meshGameObject.AddComponent<Sliceable>();

            sliceable.IsSolid = originalSliceable.IsSolid;
            sliceable.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
            sliceable.UseGravity = originalSliceable.UseGravity;

            meshGameObject.GetComponent<MeshRenderer>().materials = originalMaterial;

            meshGameObject.transform.localScale = originalObject.transform.localScale;
            meshGameObject.transform.rotation = originalObject.transform.rotation;
            meshGameObject.transform.position = originalObject.transform.position;

            meshGameObject.tag = originalObject.tag;

            return meshGameObject;
        }

        /// <summary>
        /// Add mesh collider and rigid body to game object
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="mesh"></param>
        private static void SetupCollidersAndRigidBodys(ref GameObject gameObject, Mesh mesh, bool useGravity, PhysicMaterial physicMaterial)
        {
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;
            meshCollider.material = physicMaterial;

            var rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = useGravity;
        }
    }
}