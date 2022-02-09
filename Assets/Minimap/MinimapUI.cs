using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapUI : MonoBehaviour
{
    [SerializeField] Image border;
    [SerializeField] RawImage map;
    [SerializeField] RawImage mask;

    void Update()
    {
        if (ObjectManager.Instance.player.currentMovementMode != Player.MovementMode.swimming) {
            border.enabled = false;
            map.enabled = false;
            mask.enabled = false;
        } else {
            border.enabled = true;
            map.enabled = true;
            mask.enabled = true;
        }
    }
}
