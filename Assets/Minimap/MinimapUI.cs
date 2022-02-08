using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    Image border;
    RawImage map;

    private void Start() {
        border = GetComponent<Image>();
        map = GetComponentInChildren<RawImage>();
    }

    void Update()
    {
        if (ObjectManager.Instance.player.currentMovementMode != Player.MovementMode.swimming) {
            border.enabled = false;
            map.enabled = false;
        } else {
            border.enabled = true;
            map.enabled = true;
        }
    }
}
