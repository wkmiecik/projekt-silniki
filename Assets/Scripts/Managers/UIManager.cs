using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Camera
    Camera cam;

    void Start() {
        cam = Camera.main;
    }




    // Ship ui
    [SerializeField] TMP_Text shipHpText;

    void Update() {
        shipHpText.transform.rotation = cam.transform.rotation;
    }

    public void SetShipHPtext(int hp) {
        shipHpText.text = $"{hp} HP";
    }
}
