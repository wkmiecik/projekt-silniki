using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainShip : MonoBehaviour
{
    // Access to player
    Player player;
    // Access to ui manager
    UIManager ui;

    // Ship HP
    public int HP = 100;

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
        // Access to player variables
        player = ObjectManager.Instance.player;
        // Access to ui manager
        ui = ObjectManager.Instance.uiManager;

        // Set starting hp
        ui.SetShipHPtext(HP);
    }

    void Update()
    {
        // Hide sails if player onboard
        if (player.currentMovementMode == Player.MovementMode.walkingOnShip || player.currentMovementMode == Player.MovementMode.cannonShooting) {
            sails.SetActive(false);
        } else {
            sails.SetActive(true);
        }
    }


    public void getHit(int dmg) {
        // Subtract dmg from hp
        HP -= dmg;

        // Check if still alive
        if (HP <= 0) {
            // Ded
            ui.SetShipHPtext(0);
            Debug.Log("DEAD"); // Dying just prints DEAD for now
        } else {
            // If still alive update hp text
            ui.SetShipHPtext(HP);
        }
    }
}
