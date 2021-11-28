using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DayNightCycle : MonoBehaviour
{
    GameObject sun;
    Light sunLight;

    [SerializeField]
    float cycleLength = 100;

    void Start() {
        sun = GameObject.FindGameObjectWithTag("Sun");
        sunLight = sun.GetComponent<Light>();

        sun.transform.DORotate(new Vector3(360f, 0, 0), cycleLength, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetRelative();
    }
}
