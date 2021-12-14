using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainShip : MonoBehaviour
{
    // Access to ship variables
    ShipVars sv;
    // Access to player variables
    PlayerVars pv;
    // Access to ui manager
    UIManager ui;

    // Sails
    public GameObject sails;
    public GameObject[] sailsUp;
    public GameObject[] sailsDown;

    // Spawn points
    public GameObject playerSpawnPoint;
    public GameObject boatSpawnPoint;

    // Colliders
    public Collider outsideCollider;

    void Start()
    {
        // Access to ship variables
        sv = ShipVars.Instance;
        // Access to player variables
        pv = PlayerVars.Instance;
        // Access to ui manager
        ui = ObjectManager.Instance.uiManager;

        // Set starting hp
        ui.SetShipHPtext(sv.HP);
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


    public void getHit(int dmg) {
        // Subtract dmg from hp
        sv.HP -= dmg;

        // Check if still alive
        if (sv.HP <= 0) {
            // Ded
            ui.SetShipHPtext(0);
            Debug.Log("DEAD");
            Debug.Break(); // Dying just pauses editor for now
        } else {
            // If still alive update hp text
            ui.SetShipHPtext(sv.HP);
        }
    }
}
