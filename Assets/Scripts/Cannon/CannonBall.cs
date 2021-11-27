using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [HideInInspector]
    public float shootingForce;
    Rigidbody rb;

    [SerializeField]
    int damage = 10;

    float ttl = 10f;

    void Start() {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.AddRelativeForce(Vector3.forward * shootingForce, ForceMode.Impulse);
    }

    void Update() {
        ttl -= Time.deltaTime;

        if (ttl < 0) {
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
