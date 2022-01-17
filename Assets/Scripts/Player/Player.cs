using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Access to ship
    MainShip ship;

    // Access to UI manager
    UIManager uiManager;

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
    public float normalForce = 20f;
    public float boostForce = 7f;

    [Header("Boat boost")]
    public float boostLength = 10f;
    public float boostRecoveryDelay = 2f;

    [Header("Boat rotation")]
    public float rotationLimitFactor = 2000;

    [HideInInspector] public GameObject usedCannon;


    [Header("Walking movement")]
    // Walking variables
    public float walkingForce = 5f;


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
    bool boostActive = false;


    void Start() {
        // Access to ship
        ship = ObjectManager.Instance.ship;

        // Access to UI manager
        uiManager = ObjectManager.Instance.uiManager;

        // Objects
        legsCollider = legs.GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        // Movement timers
        boostTimer = boostLength;
        boostRecoveryDelayTimer = boostRecoveryDelay;
    }

    void FixedUpdate() {
        // If ui is not blocking input, move player using current movement mode
        if (!uiManager.uiBlockingInput) {
            switch (currentMovementMode) {
                case MovementMode.swimming:
                    if (boostActive) {
                        rb.AddRelativeForce(Vector3.forward * acc * 300000 * boostForce * Time.fixedDeltaTime);
                    }
                    else {
                        rb.AddRelativeForce(Vector3.forward * acc * 300000 * normalForce * Time.fixedDeltaTime);
                    }

                    rb.AddRelativeTorque(Vector3.up * rot * 200000 * Time.fixedDeltaTime);

                    // Limit rotation so the boat doesnt flip
                    // Force should probably depend on current rotation!!!!
                    Quaternion rotation = Quaternion.FromToRotation(transform.up, Vector3.up);
                    rb.AddTorque(new Vector3(rotation.x, rotation.y, rotation.z) * rotationLimitFactor);
                    break;



                case MovementMode.walkingOnShip:
                    // Look at mouse
                    float mouseDistSqr = (mouseWorldPosition - transform.position).sqrMagnitude;
                    if (mouseDistSqr > 1) {
                        //transform.LookAt(mouseWorldPosition);
                        Vector3 direction = mouseWorldPosition - transform.position;
                        direction = Math3d.ProjectVectorOnPlane(Vector3.up, direction);
                        transform.localRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 0, 0);
                    }

                    rb.velocity = new Vector3(0, rb.velocity.y, 0);
                    rb.velocity += (Vector3.left + Vector3.forward) * acc * walkingForce;
                    rb.velocity += (Vector3.forward + Vector3.right) * rot * walkingForce;
                    break;



                case MovementMode.cannonShooting:
                    // Set player position behind cannon;
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    transform.position = usedCannon.transform.position;
                    transform.rotation = usedCannon.transform.rotation;
                    transform.Translate((Vector3.back * 1.2f) + (Vector3.up * .1f), usedCannon.transform);
                    break;
            }
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
            boostActive = true;
            boostTimer -= Time.deltaTime;
            boostRecoveryDelayTimer = boostRecoveryDelay;
        } else {
            if (boostRecoveryDelayTimer > 0) {
                boostRecoveryDelayTimer -= Time.deltaTime;
            } else {
                if (boostTimer < boostLength) boostTimer += Time.deltaTime;
            }
            boostActive = false;
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
                rb.mass = 200;
                rb.freezeRotation = false;
                if (updatePosition) {
                    gameObject.transform.position = ship.boatSpawnPoint.transform.position;
                    gameObject.transform.rotation = ship.boatSpawnPoint.transform.rotation;
                }
                legs.SetActive(false);
                boat.SetActive(true);

                ObjectManager.Instance.cameraController.SwitchToMainCamera();
                ship.SetSailsVisible();
                break;


            case MovementMode.walkingOnShip:
                currentMovementMode = MovementMode.walkingOnShip;
                rb.velocity = Vector3.zero;
                rb.mass = 50;
                rb.freezeRotation = true;
                if (updatePosition) {
                    gameObject.transform.position = ship.playerSpawnPoint.transform.position;
                    gameObject.transform.rotation = ship.playerSpawnPoint.transform.rotation;
                }
                boat.SetActive(false);
                legs.SetActive(true);

                ObjectManager.Instance.cameraController.SwitchToShipCamera();
                ship.SetSailsTransparent();
                break;


            case MovementMode.cannonShooting:
                currentMovementMode = MovementMode.cannonShooting;
                ObjectManager.Instance.cameraController.SwitchToMainCamera();
                ship.SetSailsTransparent();
                break;
        }
    }
}
