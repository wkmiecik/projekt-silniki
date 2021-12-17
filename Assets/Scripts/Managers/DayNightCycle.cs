using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DayNightCycle : MonoBehaviour
{
    // Access to sun light
    Light sun;

    // Cycle variables
    [SerializeField] bool paused = false;
    [SerializeField] float cycleLength = 200;
    [SerializeField] float timeScale = 1;

    // Cycle tween
    [HideInInspector] public Tween cycle;

    void Start() {
        // Access to Objects
        sun = ObjectManager.Instance.sun.GetComponent<Light>();
        
        // Create cycle tween
        cycle = sun.transform.DORotate(new Vector3(360f, 0, 0), cycleLength, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetRelative();
    }


    void Update() {
        // Pause tween
        if (paused) {
            cycle.Pause();
        } else {
            cycle.Play();
        }

        // Set timescale
        Time.timeScale = timeScale;
        //cycle.timeScale = timeScale;
    }
}
