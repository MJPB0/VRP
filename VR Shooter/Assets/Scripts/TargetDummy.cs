using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummy : MonoBehaviour
{
    [SerializeField] GameObject body;
    [SerializeField] Material dummyMaterialHit;
    [SerializeField] Material dummyMaterial;

    [SerializeField] float changeMaterialTime;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            body.GetComponent<MeshRenderer>().material = dummyMaterialHit;
            StartCoroutine(WaitAndChangeMaterial(changeMaterialTime));
            Destroy(collision.gameObject);
        }
    }

    IEnumerator WaitAndChangeMaterial(float time)
    {
        yield return new WaitForSeconds(time);
        body.GetComponent<MeshRenderer>().material = dummyMaterial;
    }
}
