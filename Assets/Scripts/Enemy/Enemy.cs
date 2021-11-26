using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    string targetTag;
    GameObject target;
    Rigidbody rb;

    [SerializeField]
    float speed = 10;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag(targetTag);
    }

    void FixedUpdate() {
        transform.LookAt(target.transform);
        rb.AddRelativeForce(Vector3.forward * speed * 10000 * Time.fixedDeltaTime, ForceMode.Force);
    }
}
