using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    // Access to player variables
    PlayerVars pv;

    // Access to Objects
    ObjectManager objM;

    // Camera
    CinemachineVirtualCamera cam;
    GameObject followPoint;

    void Start()
    {
        // Access to player variables
        pv = PlayerVars.Instance;

        // Access to Objects
        objM = ObjectManager.Instance;

        // Camera
        cam = GetComponent<CinemachineVirtualCamera>();
        followPoint = new GameObject("Camera follow point");
    }

    void Update()
    {
        // Set camera follow point depending on current movement mode
        switch (pv.currentMovementMode) {
            // If swimming, just follow
            case PlayerVars.MovementMode.swimming:
                followPoint.transform.position = objM.player.transform.position;
                break;

            // If walking on ship, just follow
            case PlayerVars.MovementMode.walkingOnShip:
                followPoint.transform.position = objM.ship.transform.position;
                break;

            // If using cannon, move camera slightly towards mouse position
            case PlayerVars.MovementMode.cannonShooting:
                followPoint.transform.position = pv.usedCannon.transform.position;

                float mouseX = Mathf.Clamp(Camera.main.ScreenToViewportPoint(Input.mousePosition).x, 0, 1);
                float mouseY = Mathf.Clamp(Camera.main.ScreenToViewportPoint(Input.mousePosition).y, 0, 1);
                float dist = Vector2.Distance(new Vector2(.5f, .5f), new Vector2(mouseX, mouseY));
                followPoint.transform.Translate(Vector3.forward * dist * 20, pv.usedCannon.transform);
                break;
        }

        cam.Follow = followPoint.transform;
    }
}
