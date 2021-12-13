using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainShip : MonoBehaviour
{
    // Access to player variables
    PlayerVars pv;

    // Sails
    public GameObject sails;
    public GameObject[] sailsUp;
    public GameObject[] sailsDown;

    // Spawn points
    public GameObject playerSpawnPoint;
    public GameObject boatSpawnPoint;

    void Start()
    {
        // Access to player variables
        pv = PlayerVars.Instance;
    }

    void Update()
    {
        // Hide sails if player onboard
        if (pv.currentMovementMode == PlayerVars.MovementMode.walkingOnShip || pv.currentMovementMode == PlayerVars.MovementMode.cannonShooting) {
            sails.SetActive(false);
        } else {
            sails.SetActive(true);
        }
    }
}
