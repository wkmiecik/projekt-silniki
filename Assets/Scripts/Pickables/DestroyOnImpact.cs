using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnImpact : MonoBehaviour
{
    MeshRenderer meshRenderer;
    BuoyantObj buoyantScript;
    Rigidbody[] chunksRbs;
    Rigidbody rb;

    GameObject hierarchyParent;

    [SerializeField] float hitForceMultiplayer = 1f;
    [SerializeField] float enableChunksCollisionDelay = .2f;

    bool wasAlredyHit = false;


    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        buoyantScript = GetComponent<BuoyantObj>();
        chunksRbs = GetComponentsInChildren<Rigidbody>(true);
        rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        hierarchyParent = GameObject.Find("Barrels");
    }


    private void OnCollisionEnter(Collision collision) {
        if (!wasAlredyHit) {
            wasAlredyHit = true;

            Vector3 hitForce = collision.impulse / Time.fixedDeltaTime;

            meshRenderer.enabled = false;
            buoyantScript.enabled = false;
            rb.detectCollisions = false;

            foreach (var chunk in chunksRbs) {
                chunk.transform.SetParent(hierarchyParent.transform);
                chunk.gameObject.SetActive(true);
                chunk.AddForceAtPosition(hitForce * hitForceMultiplayer, collision.contacts[0].point);

                chunk.detectCollisions = false;
                StartCoroutine(EnableCollisonsAfterSeconds(enableChunksCollisionDelay));
            }
        }
    }


    IEnumerator EnableCollisonsAfterSeconds(float delay) {
        yield return new WaitForSeconds(delay);

        foreach (var chunk in chunksRbs) {
            chunk.detectCollisions = true;
        }
    }
}
