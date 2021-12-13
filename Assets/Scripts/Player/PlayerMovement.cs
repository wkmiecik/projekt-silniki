using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player variables
    PlayerVars pv;

    // Objects
    ObjectManager objM;
    public GameObject boat;
    public GameObject legs;
    [HideInInspector] public CapsuleCollider legsCollider;
    [HideInInspector] public Rigidbody rb;

    // Input
    float rot, acc;
    Vector3 mouse = Vector3.zero;
    Ray castPoint;

    // Movement modes changing
    bool movementChangeLocked = false;

    // Movement timers
    float movementChangeDelayTimer = 0f;
    float boostTimer;
    float boostRecoveryDelayTimer;


    void Start() {
        // Player variables
        pv = PlayerVars.Instance;

        // Objects
        objM = ObjectManager.Instance;
        legsCollider = legs.GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        // Movement timers
        boostTimer = pv.boostLength;
        boostRecoveryDelayTimer = pv.boostRecoveryDelay;
    }

    void FixedUpdate() {
        RaycastHit hit;
        float mouseDistSqr;

        switch (pv.currentMovementMode) {
            // If swimming in boat
            case PlayerVars.MovementMode.swimming:
                // Basic movement forces
                rb.AddRelativeForce(Vector3.forward * acc * 150000 * Time.fixedDeltaTime);
                rb.AddRelativeTorque(Vector3.up * rot * 100000 * Time.fixedDeltaTime);

                // Limit speed using resistance force in opposite movement direction
                rb.AddForce(-rb.velocity * 150 * /*resistanceForce**/ Time.fixedDeltaTime);

                // Limit rotation so the boat doesnt flip
                // Force should probably depend on rotation!!!!
                Quaternion rotation = Quaternion.FromToRotation(transform.up, Vector3.up);
                rb.AddTorque(new Vector3(rotation.x, rotation.y, rotation.z) * pv.rotationLimitFactor);
                break;

            //  If walking on a ship
            case PlayerVars.MovementMode.walkingOnShip:
                mouse = Input.mousePosition;
                castPoint = Camera.main.ScreenPointToRay(mouse);
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, LayerMask.GetMask("MouseCollider"))) {
                    pv.mouseWorldPosition = hit.point;
                    pv.mouseWorldPosition.y = transform.position.y;

                    mouseDistSqr = (pv.mouseWorldPosition - transform.position).sqrMagnitude;

                    if (mouseDistSqr > 3) {
                        transform.LookAt(pv.mouseWorldPosition);
                    }
                }

                rb.AddForce((Vector3.left + Vector3.forward) * acc * 600000 * Time.fixedDeltaTime);
                rb.AddForce((Vector3.forward + Vector3.right) * rot * 300000 * Time.fixedDeltaTime);
                break;

            // If shooting cannon
            case PlayerVars.MovementMode.cannonShooting:
                mouse = Input.mousePosition;
                castPoint = Camera.main.ScreenPointToRay(mouse);
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, LayerMask.GetMask("MouseCollider"))) {
                    pv.mouseWorldPosition = hit.point;
                    pv.mouseWorldPosition.y = pv.usedCannon.transform.position.y;

                    mouseDistSqr = (pv.mouseWorldPosition - transform.position).sqrMagnitude;

                    if (mouseDistSqr > 3) {
                        pv.usedCannon.transform.LookAt(pv.mouseWorldPosition);
                    }
                }

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                transform.position = pv.usedCannon.transform.position;
                transform.rotation = pv.usedCannon.transform.rotation;
                transform.Translate((Vector3.back * 1.6f) + (Vector3.up * .4f), pv.usedCannon.transform);
                break;
        }
    }


    private void Update() {
        // Read inputs
        rot = Input.GetAxis("Horizontal");
        acc = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && boostTimer > 0) {
            pv.resistanceForce = pv.boostResistanceForce;
            boostTimer -= Time.deltaTime;
            boostRecoveryDelayTimer = pv.boostRecoveryDelay;
        } else {
            if (boostRecoveryDelayTimer > 0) {
                boostRecoveryDelayTimer -= Time.deltaTime;
            } else {
                if (boostTimer < pv.boostLength) boostTimer += Time.deltaTime;
            }
            pv.resistanceForce = pv.normalResistanceForce;
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
                SwitchMovementMode(PlayerVars.MovementMode.walkingOnShip, true);
            } else {
                SwitchMovementMode(PlayerVars.MovementMode.swimming, true);
            }
        }

        if (Input.GetKey(KeyCode.E) && other.tag == "Cannon" && pv.currentMovementMode == PlayerVars.MovementMode.walkingOnShip && !movementChangeLocked) {
            movementChangeLocked = true;
            movementChangeDelayTimer = 1f;
            legsCollider.enabled = false;

            // Activate Cannon
            pv.usedCannon = other.gameObject;
            var usedCannonScript = pv.usedCannon.GetComponent<Cannon>();
            usedCannonScript.active = true;

            SwitchMovementMode(PlayerVars.MovementMode.cannonShooting, true);
        }

        if (Input.GetKey(KeyCode.E) && pv.currentMovementMode == PlayerVars.MovementMode.cannonShooting && !movementChangeLocked) {
            movementChangeLocked = true;
            movementChangeDelayTimer = 1f;
            legsCollider.enabled = true;

            // Deactivate Cannon
            var usedCannonScript = pv.usedCannon.GetComponent<Cannon>();
            usedCannonScript.active = false;

            SwitchMovementMode(PlayerVars.MovementMode.walkingOnShip, false);
        }
    }


    private void SwitchMovementMode(PlayerVars.MovementMode movementMode, bool updatePosition) {
        switch (movementMode) {
            case PlayerVars.MovementMode.swimming:
                pv.currentMovementMode = PlayerVars.MovementMode.swimming;
                rb.velocity = Vector3.zero;
                if (updatePosition) {
                    gameObject.transform.position = objM.ship.boatSpawnPoint.transform.position;
                    gameObject.transform.rotation = objM.ship.boatSpawnPoint.transform.rotation;
                }
                legs.SetActive(false);
                boat.SetActive(true);
                break;


            case PlayerVars.MovementMode.walkingOnShip:
                pv.currentMovementMode = PlayerVars.MovementMode.walkingOnShip;
                rb.velocity = Vector3.zero;
                if (updatePosition) {
                    gameObject.transform.position = objM.ship.playerSpawnPoint.transform.position;
                    gameObject.transform.rotation = objM.ship.playerSpawnPoint.transform.rotation;
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
