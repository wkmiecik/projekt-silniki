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
    [SerializeField]
    float slowSpeed = 5;
    [HideInInspector]
    public bool slowed = false;

    [SerializeField]
    public int hp = 100;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag(targetTag);
    }

    void FixedUpdate() {
        // Go to target
        transform.LookAt(target.transform);
        Vector3 force = Vector3.forward * speed * 10000;
        rb.AddRelativeForce(force * Time.fixedDeltaTime, ForceMode.Force);

        if (slowed) {
            force = Vector3.back * slowSpeed * 10000;
            rb.AddRelativeForce(force * Time.fixedDeltaTime, ForceMode.Force);
        }

        // Check if alive
        if (hp <= 0) {
            Destroy(gameObject);
        }
    }
}
