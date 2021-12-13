using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveSpawner : MonoBehaviour
{
    // Access to Objects
    ObjectManager objM;

    // Enemy prefab
    [SerializeField] GameObject enemyPrefeb;

    // [0-1] Day starts ~0, night starts ~.5
    float cyclePercentage;
    [SerializeField] float nightStart = .47f;
    [SerializeField] float nightEnd = .80f;

    // List of spawners
    GameObject[] spawners;

    // Enemy spawning variables
    bool spawning = false;
    [SerializeField] float spawningDelay = 6f;
    float spawningDelayTimer;


    void Start() {
        // Access to Objects
        objM = ObjectManager.Instance;

        // List of spawners
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
    }


    void Update()
    {
        // Get current cycle progress from day/night cycle manager
        cyclePercentage = objM.dayNightCycle.cycle.ElapsedDirectionalPercentage();

        // Check if night
        if (cyclePercentage > nightStart && cyclePercentage < nightEnd) {
            spawning = true;
            spawningDelayTimer -= Time.deltaTime;
        } else {
            spawning = false;
            spawningDelayTimer = spawningDelay;
        }

        // Spawn enemy at every spawner when delay elapsed
        if (spawning && spawningDelayTimer < 0) {
            spawningDelayTimer = spawningDelay;

            foreach(GameObject spawner in spawners) {
                SpawnEnemy(spawner.transform);
            }
        }
    }

    void SpawnEnemy(Transform spawner) {
        Instantiate(enemyPrefeb, spawner.position, spawner.rotation);
    }
}
