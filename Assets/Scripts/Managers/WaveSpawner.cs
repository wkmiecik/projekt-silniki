using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefeb;
    [SerializeField] DayNightCycle dayNightCycle;

    // [0-1] Day starts ~0, night starts ~.5
    float cyclePercentage;

    [SerializeField] float nightStart = .47f;
    [SerializeField] float nightEnd = .80f;

    GameObject[] spawners;
    bool spawning = false;
    [SerializeField] float spawningDelay = 6f;
    float spawningDelayTimer;


    void Start() {
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
    }


    void Update()
    {
        cyclePercentage = dayNightCycle.cycle.ElapsedDirectionalPercentage();

        if (cyclePercentage > nightStart && cyclePercentage < nightEnd) {
            spawning = true;
            spawningDelayTimer -= Time.deltaTime;
        } else {
            spawning = false;
            spawningDelayTimer = spawningDelay;
        }

        if (spawning && spawningDelayTimer < 0) {
            spawningDelayTimer = spawningDelay;

            foreach(GameObject spawner in spawners) {
                SpawnEnemy(spawner.transform);
            }
        }
    }

    void SpawnEnemy(Transform spawner) {
        Debug.Log("SPAWN");
        Instantiate(enemyPrefeb, spawner.position, spawner.rotation);
    }
}
