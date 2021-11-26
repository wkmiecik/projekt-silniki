using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField]
    GameObject cannonBallPrefab;

    [SerializeField]
    Transform cannonBallSpawnPoint;

    [SerializeField]
    float shootingForce = 500f;

    [SerializeField]
    float shootingDelay = .1f;
    float shootingTimer = 0;

    void Update()
    {
        shootingTimer -= Time.deltaTime;

        if (Input.GetKey(KeyCode.Mouse0) && shootingTimer <= 0) {
            Shoot();
        }
    }

    void Shoot() {
        shootingTimer = shootingDelay;
        GameObject ballObj = Instantiate(cannonBallPrefab, cannonBallSpawnPoint.position, cannonBallSpawnPoint.rotation);
        CannonBall cannonBall = ballObj.GetComponent<CannonBall>();
        cannonBall.shootingForce = shootingForce;
    }
}
