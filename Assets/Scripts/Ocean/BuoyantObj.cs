using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class BuoyantObj : MonoBehaviour {
    // Access to ocean manager
    OceanManager oceanManager;

    [Header("Floaters")]
    public Transform[] floaters;

    [Header("Buoyancy settings")]
    public float underWaterDrag = 3f;

    public float underWaterAngularDrag = 1f;

    public float airDrag = 0f;

    public float airAngularDrag = 0.05f;

    public float floatingPower = 15f;

    int floatersUnderwater;

    bool underwater;

    Rigidbody rb;

    // Optimization
    [Header("Optimizations")]
    [SerializeField] bool buoyancyOffWhenFarFromPlayer = false;
    [SerializeField] float buoyancyOffDistanceSqr = 1000;
    // Access to player
    Player player;


    void Start() {
        // Access to ocean manager
        oceanManager = ObjectManager.Instance.oceanManager;

        // Access to player
        player = ObjectManager.Instance.player;

        rb = GetComponent<Rigidbody>();
    }


    //DEBUGGING
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;

        if (floaters.Length > 0) {
            foreach (var floater in floaters) {
                Gizmos.DrawSphere(floater.position, .1f);
            }
        }


        if (Application.IsPlaying(this)) {
            Gizmos.color = Color.red;
            foreach (var floater in floaters) {
                Gizmos.DrawSphere(new Vector3(floater.position.x, oceanManager.GetHeightAtPosition(floater.position), floater.position.z), .3f);
            }
        }
    }


    void FixedUpdate() {
        floatersUnderwater = 0;

        Vector2 this2dPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 player2dPos = new Vector2(player.transform.position.x, player.transform.position.z);

        if (!buoyancyOffWhenFarFromPlayer || (this2dPos - player2dPos).sqrMagnitude < buoyancyOffDistanceSqr) {
            rb.isKinematic = false;

            for (int i = 0; i < floaters.Length; i++) {
                float difference = floaters[i].position.y - oceanManager.GetHeightAtPosition(floaters[i].position);

                if (difference < 0) {
                    rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floaters[i].position + (Vector3.up * 2), ForceMode.Acceleration);
                    floatersUnderwater += 1;

                    if (!underwater) {
                        underwater = true;
                        SwitchState(true);
                    }
                }

                if (underwater && floatersUnderwater == 0) {
                    underwater = false;
                    SwitchState(false);
                }
            }
        } else {
            rb.isKinematic = true;
        }


        void SwitchState(bool isUnderwater) {
            if (isUnderwater) {
                rb.drag = underWaterDrag;
                rb.angularDrag = underWaterAngularDrag;

            }
            else {
                rb.drag = airDrag;
                rb.angularDrag = airAngularDrag;
            }
        }
    }
}