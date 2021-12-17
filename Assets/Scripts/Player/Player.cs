using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Access to ship
    MainShip ship;

    // Movement modes
    public enum MovementMode {
        swimming,
        walkingOnShip,
        cannonShooting
    }

    [Header("Movement modes")]
    // Current movement mode
    public MovementMode currentMovementMode = MovementMode.swimming;

    [Header("Boat movement")]
    // Boat movement variables
    public float resistanceForce = 0f;
    public float normalResistanceForce = 20f;
    public float boostResistanceForce = 7f;

    [Header("Boat boost")]
    public float boostLength = 10f;
    public float boostRecoveryDelay = 2f;

    [Header("Boat rotation")]
    public float rotationLimitFactor = 2000;

    [HideInInspector] public GameObject usedCannon;

    [Header("Objects")]
    // Objects
    public GameObject boat;
    public GameObject legs;
    [HideInInspector] public CapsuleCollider legsCollider;
    [HideInInspector] public Rigidbody rb;

    // Input
    float rot, acc;
    [HideInInspector] public Vector3 mouseWorldPosition;

    // Movement modes changing
    bool movementChangeLocked = false;

    // Movement timers
    float movementChangeDelayTimer = 0f;
    float boostTimer;
    float boostRecoveryDelayTimer;


    void Start() {
        // Access to ship
        ship = ObjectManager.Instance.ship;

        // Objects
        legsCollider = legs.GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        // Movement timers
        boostTimer = boostLength;
        boostRecoveryDelayTimer = boostRecoveryDelay;
    }

    void FixedUpdate() {
        RaycastHit hit;
        float mouseDistSqr;

        switch (currentMovementMode) {
            // If swimming in boat
            case MovementMode.swimming:
                // Basic movement forces
                rb.AddRelativeForce(Vector3.forward * acc * 150000 * Time.fixedDeltaTime);
                rb.AddRelativeTorque(Vector3.up * rot * 100000 * Time.fixedDeltaTime);

                // Limit speed using resistance force in opposite movement direction
                rb.AddForce(-rb.velocity * 150 * /*resistanceForce**/ Time.fixedDeltaTime);

                // Limit rotation so the boat doesnt flip
                // Force should probably depend on rotation!!!!
                Quaternion rotation = Quaternion.FromToRotation(transform.up, Vector3.up);
                rb.AddTorque(new Vector3(rotation.x, rotation.y, rotation.z) * rotationLimitFactor);
                break;

            //  If walking on a ship
            case MovementMode.walkingOnShip:
                var mouse = Input.mousePosition;
                var castPoint = Camera.main.ScreenPointToRay(mouse);
                if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, LayerMask.GetMask("MouseCollider"))) {
                    mouseWorldPosition = hit.point;
                    mouseWorldPosition.y = transform.position.y;

                    mouseDistSqr = (mouseWorldPosition - transform.position).sqrMagnitude;

                    if (mouseDistSqr > 3) {
                        transform.LookAt(mouseWorldPosition);
                    }
                }

                rb.AddForce((Vector3.left + Vector3.forward) * acc * 600000 * Time.fixedDeltaTime);
                rb.AddForce((Vector3.forward + Vector3.right) * rot * 300000 * Time.fixedDeltaTime);
                break;

            // If shooting cannon
            case MovementMode.cannonShooting:
                // Set player position behind cannon;
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
        var mouse = Input.mousePosition;
        var castPoint = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, LayerMask.GetMask("MouseCollider"))) {
            mouseWorldPosition = hit.point;
            //mouseWorldPosition.y = transform.position.y;
        }

        // Apply boost to boat
        if (Input.GetKey(KeyCode.LeftShift) && boostTimer > 0) {
            resistanceForce = boostResistanceForce;
            boostTimer -= Time.deltaTime;
            boostRecoveryDelayTimer = boostRecoveryDelay;
        } else {
            if (boostRecoveryDelayTimer > 0) {
                boostRecoveryDelayTimer -= Time.deltaTime;
            } else {
                if (boostTimer < boostLength) boostTimer += Time.deltaTime;
            }
            resistanceForce = normalResistanceForce;
        }

        // Delay between changing movement style
        if (movementChangeDelayTimer > 0) {
            movementChangeDelayTimer -= Time.deltaTime;
        } else {
            movementChangeLocked = false;
        }
    }


    private void OnTriggerStay(Collider other) {
        if (Input.GetKey(KeyCode.Q) && other.tag == "MainShip" && currentMovementMode != MovementMode.cannonShooting && !movementChangeLocked) {
            movementChangeLocked = true;
            movementChangeDelayTimer = 1f;
            if (currentMovementMode == MovementMode.swimming) {
                SwitchMovementMode(MovementMode.walkingOnShip, true);
            } else {
                SwitchMovementMode(MovementMode.swimming, true);
            }
        }

        if (Input.GetKey(KeyCode.E) && other.tag == "Cannon" && currentMovementMode == MovementMode.walkingOnShip && !movementChangeLocked) {
            movementChangeLocked = true;
            movementChangeDelayTimer = 1f;
            legsCollider.enabled = false;

            // Activate Cannon
            usedCannon = other.gameObject;
            var usedCannonScript = usedCannon.GetComponent<Cannon>();
            usedCannonScript.playerSteering = true;

            SwitchMovementMode(MovementMode.cannonShooting, true);
        }

        if (Input.GetKey(KeyCode.E) && currentMovementMode == MovementMode.cannonShooting && !movementChangeLocked) {
            movementChangeLocked = true;
            movementChangeDelayTimer = 1f;
            legsCollider.enabled = true;

            // Deactivate Cannon
            var usedCannonScript = usedCannon.GetComponent<Cannon>();
            usedCannonScript.playerSteering = false;

            SwitchMovementMode(MovementMode.walkingOnShip, false);
        }
    }


    private void SwitchMovementMode(MovementMode movementMode, bool updatePosition) {
        switch (movementMode) {
            case MovementMode.swimming:
                currentMovementMode = MovementMode.swimming;
                rb.velocity = Vector3.zero;
                if (updatePosition) {
                    gameObject.transform.position = ship.boatSpawnPoint.transform.position;
                    gameObject.transform.rotation = ship.boatSpawnPoint.transform.rotation;
                }
                legs.SetActive(false);
                boat.SetActive(true);
                break;


            case MovementMode.walkingOnShip:
                currentMovementMode = MovementMode.walkingOnShip;
                rb.velocity = Vector3.zero;
                if (updatePosition) {
                    gameObject.transform.position = ship.playerSpawnPoint.transform.position;
                    gameObject.transform.rotation = ship.playerSpawnPoint.transform.rotation;
                }
                boat.SetActive(false);
                legs.SetActive(true);
                break;


            case MovementMode.cannonShooting:
                currentMovementMode = MovementMode.cannonShooting;
                break;
        }
    }
}
