using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    MeshRenderer meshRenderer;
    BuoyantObj buoyantScript;
    Rigidbody[] chunksRbs;

    private void Start() {
        meshRenderer = GetComponent <MeshRenderer>();
        buoyantScript = GetComponent <BuoyantObj>();
        chunksRbs = GetComponentsInChildren<Rigidbody>(true);
    }

    private void OnCollisionEnter(Collision collision) {
        Vector3 hitForce = collision.impulse / Time.fixedDeltaTime;

        meshRenderer.enabled = false;
        buoyantScript.enabled = false;

        foreach (var chunk in chunksRbs) {
            chunk.transform.SetParent(null, true);
            chunk.gameObject.SetActive(true);
        }
    }
}
