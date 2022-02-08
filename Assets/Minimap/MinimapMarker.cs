using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapMarker : MonoBehaviour
{
    [SerializeField] GameObject targetToFollow;
    Quaternion rotation;

    void Start() {
        rotation = Quaternion.Euler(90,0,45);
    }

    void Update()
    {
        if (targetToFollow != null) {
            transform.position = targetToFollow.transform.position;
            rotation = Quaternion.Euler(90f, targetToFollow.transform.eulerAngles.y, 0);
        }
        transform.rotation = rotation;
    }
}
