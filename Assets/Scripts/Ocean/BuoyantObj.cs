using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class BuoyantObj : MonoBehaviour {
    // Access to ocean manager
    OceanManager oceanManager;

    public Transform[] floaters;

    public float underWaterDrag = 3f;

    public float underWaterAngularDrag = 1f;

    public float airDrag = 0f;

    public float airAngularDrag = 0.05f;

    public float floatingPower = 15f;

    Rigidbody m_Rigidbody;

    int floatersUnderwater;

    bool underwater;


    // Start is called before the first frame update
    void Start() {
        // Access to ocean manager
        oceanManager = ObjectManager.Instance.oceanManager;

        m_Rigidbody = GetComponent<Rigidbody>();
    }


    //DEBUGGING
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        foreach (var floater in floaters) {
            Gizmos.DrawSphere(floater.position, .1f);
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

        for (int i = 0; i < floaters.Length; i++) {
            float difference = floaters[i].position.y - oceanManager.GetHeightAtPosition(floaters[i].position);

            if (difference < 0) {
                m_Rigidbody.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floaters[i].position + (Vector3.up*2), ForceMode.Acceleration);
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


        void SwitchState(bool isUnderwater) {
            if (isUnderwater) {
                m_Rigidbody.drag = underWaterDrag;
                m_Rigidbody.angularDrag = underWaterAngularDrag;

            }
            else {
                m_Rigidbody.drag = airDrag;
                m_Rigidbody.angularDrag = airAngularDrag;
            }
        }
    }
}