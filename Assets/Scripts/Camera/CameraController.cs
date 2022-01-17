using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    // Access to player
    Player player;

    // Access to ship
    MainShip ship;

    // Cameras
    [SerializeField] CinemachineVirtualCamera mainCam;
    [SerializeField] CinemachineVirtualCamera shipCam;
    [SerializeField] CinemachineVirtualCamera shipMenuCam;
    GameObject followPoint;

    void Start()
    {
        // Access to player variables
        player = ObjectManager.Instance.player;

        // Access to ship
        ship = ObjectManager.Instance.ship;

        // Camera follow point
        followPoint = new GameObject("Camera follow point");
    }

    void Update()
    {
        // Set camera follow point depending on current movement mode
        switch (player.currentMovementMode) {
            // If swimming, just follow
            case Player.MovementMode.swimming:
                followPoint.transform.position = player.transform.position;
                break;

            // If walking on ship, just follow
            case Player.MovementMode.walkingOnShip:
                followPoint.transform.position = ship.transform.position;
                break;

            // If using cannon, move camera slightly towards mouse position
            case Player.MovementMode.cannonShooting:
                followPoint.transform.position = player.usedCannon.transform.position;

                float mouseX = Mathf.Clamp(Camera.main.ScreenToViewportPoint(Input.mousePosition).x, 0, 1);
                float mouseY = Mathf.Clamp(Camera.main.ScreenToViewportPoint(Input.mousePosition).y, 0, 1);
                float dist = Vector2.Distance(new Vector2(.5f, .5f), new Vector2(mouseX, mouseY));
                followPoint.transform.Translate((player.mouseWorldPosition - player.usedCannon.transform.position).normalized * dist * 25);
                break;
        }

        mainCam.Follow = followPoint.transform;
    }


    public void SwitchToMainCamera() {
        shipMenuCam.Priority = 1;
        shipCam.Priority = 1;
        mainCam.Priority = 10;
    }

    public void SwitchToShipCamera() {
        shipCam.Priority = 10;
        shipMenuCam.Priority = 1;
        mainCam.Priority = 1;
    }

    public void SwitchToShipMenuCamera() {
        shipMenuCam.Priority = 10;
        shipCam.Priority = 1;
        mainCam.Priority = 1;
    }
}
