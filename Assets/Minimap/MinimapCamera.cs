using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] GameObject followTarget;

    void Update()
    {
        transform.position = new Vector3( followTarget.transform.position.x, 100, followTarget.transform.position.z);
    }
}
