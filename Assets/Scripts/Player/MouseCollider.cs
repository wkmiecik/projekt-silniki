using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCollider : MonoBehaviour
{
    GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        transform.position = player.transform.position;
    }
}
