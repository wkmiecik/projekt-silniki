using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Objects
    GameObject boat;
    GameObject legs;
    GameObject ship;
    CapsuleCollider legsCollider;
    GameObject usedCannon;
    Cannon usedCannonScript;
    Rigidbody rb;


    // Input
    float rot, acc;
    Vector3 mouse = Vector3.zero;
    Ray castPoint;
    Vector3 mouseWorldPosition;


    // Movement modes changing
    float movementChangeDelayTimer = 0f;
    bool movementChangeLocked = false;


    // Movement
    [SerializeField] PlayerVars pv;
    float resistanceForce;
    float boostTimer;
    float boostRecoveryDelayTimer;


    // Spawn Points
    GameObject playerSpawnPoint;
    GameObject boatSpawnPoint;


    void Start() {
        boat = GameObject.FindGameObjectWithTag("Boat");
        legs = transform.Find("Legs").gameObject;
        ship = GameObject.FindGameObjectWithTag("MainShip");
        legsCollider = legs.GetComponent<CapsuleCollider>();
        rb = gameObject.GetComponent<Rigidbody>();

        playerSpawnPoint = ship.transform.Find("Player spawn point").gameObject;
        boatSpawnPoint = ship.transform.Find("Boat spawn point").gameObject;

        boostTimer = pv.boostLength;
        boostRecoveryDelayTimer = pv.boostRecoveryDelay;
    }

    void FixedUpdate() {
        RaycastHit hit;
        float mouseDistSqr;

        switch (pv.currentMovementMode) {
            case PlayerVars.MovementMode.swimming:
                rb.AddRelativeForce(Vector3.forward * acc * 150000 * Time.fixedDeltaTime);
                rb.AddRelativeTorque(Vector3.up * rot * 3000000 * Time.fixedDeltaTime);

                // Speed limit
                rb.AddForce(-rb.velocity * 150 * resistanceForce * Time.fixedDeltaTime);
                break;


            case PlayerVars.MovementMode.walking:
                mouse = Input.mousePosition;
                castPoint = Camera.main.ScreenPointToRay(mouse);
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, LayerMask.GetMask("MouseCollider"))) {
                    mouseWorldPosition = hit.point;
                    mouseWorldPosition.y = transform.position.y;

                    mouseDistSqr = (mouseWorldPosition - transform.position).sqrMagnitude;

                    if (mouseDistSqr > 3) {
                        transform.LookAt(mouseWorldPosition);
                    }
                }

                rb.AddForce((Vector3.left + Vector3.forward) * acc * 150000 * Time.fixedDeltaTime);
                rb.AddForce((Vector3.forward + Vector3.right) * rot * 150000 * Time.fixedDeltaTime);
                break;


            case PlayerVars.MovementMode.cannonShooting:
                mouse = Input.mousePosition;
                castPoint = Camera.main.ScreenPointToRay(mouse);
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, LayerMask.GetMask("MouseCollider"))) {
                    mouseWorldPosition = hit.point;
                    mouseWorldPosition.y = usedCannon.transform.position.y;

                    mouseDistSqr = (mouseWorldPosition - transform.position).sqrMagnitude;

                    if (mouseDistSqr > 3) {
                        usedCannon.transform.LookAt(mouseWorldPosition);
                    }
                }

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                transform.position = usedCannon.transform.position;
                transform.rotation = usedCannon.transform.rotation;
                transform.Translate((Vector3.back * 1.6f) + (Vector3.up * .4f), usedCannon.transform);
                break;
        }
    }


    private void Update() {
        // Read inputs
        rot = Input.GetAxis("Horizontal");
        acc = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && boostTimer > 0) {
            resistanceForce = pv.boostResistanceForce;
            boostTimer -= Time.deltaTime;
            boostRecoveryDelayTimer = pv.boostRecoveryDelay;
        } else {
            if (boostRecoveryDelayTimer > 0) {
                boostRecoveryDelayTimer -= Time.deltaTime;
            } else {
                if (boostTimer < pv.boostLength) boostTimer += Time.deltaTime;
            }
            resistanceForce = pv.normalResistanceForce;
        }

        // Delay between changing movement style
        if (movementChangeDelayTimer > 0) {
            movementChangeDelayTimer -= Time.deltaTime;
        } else {
            movementChangeLocked = false;
        }
    }


    private void OnTriggerStay(Collider other) {
        if (Input.GetKey(KeyCode.Q) && other.tag == "MainShip" && pv.currentMovementMode != PlayerVars.MovementMode.cannonShooting && !movementChangeLocked) {
            movementChangeLocked = true;
            movementChangeDelayTimer = 1f;
            if (pv.currentMovementMode == PlayerVars.MovementMode.swimming) {
                SwitchMovementMode(PlayerVars.MovementMode.walking, true);
            } else {
                SwitchMovementMode(PlayerVars.MovementMode.swimming, true);
            }
        }

        if (Input.GetKey(KeyCode.E) && other.tag == "MainCannon" && pv.currentMovementMode == PlayerVars.MovementMode.walking && !movementChangeLocked) {
            movementChangeLocked = true;
            movementChangeDelayTimer = 1f;
            legsCollider.enabled = false;

            // Activate Cannon
            usedCannon = other.gameObject;
            usedCannonScript = usedCannon.GetComponent<Cannon>();
            usedCannonScript.enabled = true;

            SwitchMovementMode(PlayerVars.MovementMode.cannonShooting, true);
        }

        if (Input.GetKey(KeyCode.E) && pv.currentMovementMode == PlayerVars.MovementMode.cannonShooting && !movementChangeLocked) {
            movementChangeLocked = true;
            movementChangeDelayTimer = 1f;
            legsCollider.enabled = true;

            // Deactivate Cannon
            usedCannonScript.enabled = false;

            SwitchMovementMode(PlayerVars.MovementMode.walking, false);
        }
    }


    private void SwitchMovementMode(PlayerVars.MovementMode movementMode, bool updatePosition) {
        switch (movementMode) {
            case PlayerVars.MovementMode.swimming:
                pv.currentMovementMode = PlayerVars.MovementMode.swimming;
                rb.velocity = Vector3.zero;
                if (updatePosition) {
                    gameObject.transform.position = boatSpawnPoint.transform.position;
                    gameObject.transform.rotation = boatSpawnPoint.transform.rotation;
                }
                legs.SetActive(false);
                boat.SetActive(true);
                break;


            case PlayerVars.MovementMode.walking:
                pv.currentMovementMode = PlayerVars.MovementMode.walking;
                rb.velocity = Vector3.zero;
                if (updatePosition) {
                    gameObject.transform.position = playerSpawnPoint.transform.position;
                    gameObject.transform.rotation = playerSpawnPoint.transform.rotation;
                }
                boat.SetActive(false);
                legs.SetActive(true);
                break;


            case PlayerVars.MovementMode.cannonShooting:
                pv.currentMovementMode = PlayerVars.MovementMode.cannonShooting;
                break;
        }
    }
}
