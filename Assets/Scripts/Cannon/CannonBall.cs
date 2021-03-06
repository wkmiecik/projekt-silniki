using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    Rigidbody rb;

    [HideInInspector] public float shootingForce;

    [HideInInspector] public int damage = 1;

    [HideInInspector] public float konckback = 1;

    float destroyTimer = 10f;

    void Start() {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.mass = konckback;
        rb.AddRelativeForce(Vector3.forward * shootingForce, ForceMode.VelocityChange);
    }

    void Update() {
        destroyTimer -= Time.deltaTime;

        if (destroyTimer < 0) {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy") {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.hp -= damage;
        }

        Destroy(gameObject);
    }
}
