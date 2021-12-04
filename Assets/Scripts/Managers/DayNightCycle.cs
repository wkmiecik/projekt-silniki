using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DayNightCycle : MonoBehaviour
{
    GameObject sunObj;

    [SerializeField] float cycleLength = 200;
    [SerializeField] float timeScale = 1;
    [SerializeField] bool paused = false;

    [HideInInspector] public Tween cycle;

    void Start() {
        sunObj = GameObject.FindGameObjectWithTag("Sun");

        cycle = sunObj.transform.DORotate(new Vector3(360f, 0, 0), cycleLength, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetRelative();
    }

    void Update() {
        if (paused) {
            cycle.Pause();
        } else {
            cycle.Play();
        }

        cycle.timeScale = timeScale;
    }
}
