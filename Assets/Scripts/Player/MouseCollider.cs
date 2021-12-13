using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCollider : MonoBehaviour
{
    // Access to Objects
    ObjectManager objM;

    void Start()
    {
        // Access to Objects
        objM = ObjectManager.Instance;
    }

    void FixedUpdate()
    {
        transform.position = objM.player.transform.position;
    }
}
