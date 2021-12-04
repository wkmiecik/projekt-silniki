using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowField : MonoBehaviour
{
    [SerializeField] GameObject target;

    void Update()
    {
        transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
    }

    private void OnTriggerStay(Collider other) {
        // If enemy in slow field
        if (other.tag == "Enemy") {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.slowed = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        // If enemy exit slow field
        if (other.tag == "Enemy") {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.slowed = false;
        }
    }
}
