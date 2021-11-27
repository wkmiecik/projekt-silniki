using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    GameObject boat;
    GameObject legs;
    CapsuleCollider legsCollider;
    GameObject mainShip;

    GameObject usedCannon;
    Cannon usedCannonScript;

    float rot, acc;
    Vector3 mouse = Vector3.zero;
    Ray castPoint;

    Rigidbody rb;

    float movementChangeDelayTimer = 0f;
    bool movementChangeLocked = false;

    Vector3 mouseWorldPosition;

    enum MovementMode {
        swimming,
        walking,
        cannonShooting
    }
    MovementMode currentMovementMode = MovementMode.swimming;

    [SerializeField]
    float normalResistanceForce = 15f;
    [SerializeField]
    float boostResistanceForce = 10f;
    float resistanceForce;
    [SerializeField]
    float boostLength = 10f;
    float boostTimer;
    [SerializeField]
    float boostRecoveryDelay = 1f;
    float boostRecoveryDelayTimer;



    void Start() {
        boat = GameObject.FindGameObjectWithTag("Boat");
        legs = transform.Find("Legs").gameObject;
        legsCollider = legs.GetComponent<CapsuleCollider>();
        mainShip = GameObject.FindGameObjectWithTag("MainShip");
        rb = gameObject.GetComponent<Rigidbody>();

        boostTimer = boostLength;
        boostRecoveryDelayTimer = boostRecoveryDelay;
    }

    void FixedUpdate() {
        RaycastHit hit;
        float mouseDistSqr;

        switch (currentMovementMode) {
            case MovementMode.swimming:
                rb.AddRelativeForce(Vector3.left * acc * 150000 * Time.fixedDeltaTime);
                rb.AddRelativeTorque(Vector3.up * rot * 350000 * Time.fixedDeltaTime);

                // Speed limit
                rb.AddForce(-rb.velocity * 150 * resistanceForce * Time.fixedDeltaTime);
                break;


            case MovementMode.walking:
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


            case MovementMode.cannonShooting:
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
                transform.Translate((Vector3.back * 3) + (Vector3.up * .4f), usedCannon.transform);
                break;
        }
    }

    private void Update() {
        // Read inputs
        rot = Input.GetAxis("Horizontal");
        acc = Input.GetAxis("Vertical");

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
                SwitchMovementMode(MovementMode.walking, true);
            } else {
                SwitchMovementMode(MovementMode.swimming, true);
            }
        }

        if (Input.GetKey(KeyCode.E) && other.tag == "MainCannon" && currentMovementMode == MovementMode.walking && !movementChangeLocked) {
            movementChangeLocked = true;
            movementChangeDelayTimer = 1f;
            legsCollider.enabled = false;

            // Activate Cannon
            usedCannon = other.gameObject;
            usedCannonScript = usedCannon.GetComponent<Cannon>();
            usedCannonScript.enabled = true;

            SwitchMovementMode(MovementMode.cannonShooting, true);
        }

        if (Input.GetKey(KeyCode.E) && currentMovementMode == MovementMode.cannonShooting && !movementChangeLocked) {
            movementChangeLocked = true;
            movementChangeDelayTimer = 1f;
            legsCollider.enabled = true;

            // Deactivate Cannon
            usedCannonScript.enabled = false;

            SwitchMovementMode(MovementMode.walking, false);
        }
    }

    private void SwitchMovementMode(MovementMode movementMode, bool updatePosition) {
        switch (movementMode) {
            case MovementMode.swimming:
                currentMovementMode = MovementMode.swimming;
                rb.velocity = Vector3.zero;
                if (updatePosition) {
                    gameObject.transform.position = mainShip.transform.position;
                    gameObject.transform.Translate(Vector3.back * 6, mainShip.transform);
                    gameObject.transform.rotation = mainShip.transform.rotation;
                }
                legs.SetActive(false);
                boat.SetActive(true);
                break;


            case MovementMode.walking:
                currentMovementMode = MovementMode.walking;
                rb.velocity = Vector3.zero;
                if (updatePosition) {
                    gameObject.transform.position = mainShip.transform.position;
                    gameObject.transform.Translate(Vector3.up * 3);
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
