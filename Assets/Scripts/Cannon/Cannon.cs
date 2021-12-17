using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using DG.Tweening;

public class Cannon : MonoBehaviour
{
    // Access to Waves Manager
    WavesManager wavesManager;

    public bool playerSteering = false;
    public bool aiSteering = false;

    Vector3 aimingTarget;

    [SerializeField] GameObject cannonBallPrefab;

    Transform barrel;
    Transform cannonBallSpawnPoint;

    [SerializeField] float shootingForce = 30f;

    [SerializeField] float shootingDelay = .1f;
    float shootingTimer = 0;

    VisualEffect cannonVFX;
    Light cannonLight;

    [SerializeField] Material blankMat;

    private void Start() {
        // Access to Waves Manager
        wavesManager = ObjectManager.Instance.wavesManager;

        cannonVFX = GetComponentInChildren<VisualEffect>();
        cannonLight = GetComponentInChildren<Light>();

        barrel = transform.Find("Barrel");
        cannonBallSpawnPoint = barrel.Find("SpawnPoint");
    }

    void Update()
    {
        // Reload timer counting
        shootingTimer -= Time.deltaTime;

        // If player steering cannon
        if (playerSteering) {
            // Set aiming target to mouse position
            aimingTarget = ObjectManager.Instance.player.mouseWorldPosition;
            Debug.Log(ObjectManager.Instance.player.mouseWorldPosition);

            // Aim cannon at target
            AimCannonAtTarget(aimingTarget);

            // Wait for click and shoot if reloaded
            if (Input.GetKey(KeyCode.Mouse0) && shootingTimer <= 0) {
                Shoot();
            }
        }

        // If ai steering cannon
        if (aiSteering) {
            // Check if any enemies alive
            if (wavesManager.enemiesAlive.Count > 0) {
                // Set aiming target to closest enemy
                aimingTarget = FindClosestEnemy().transform.position;

                // Aim cannon at target
                bool inRange = AimCannonAtTarget(aimingTarget);

                // Shoot if enemy in range and reloaded
                if (inRange && shootingTimer <= 0) {
                    Shoot();
                }
            }
        }


        //if (playerSteering || aiSteering) {
        //    if (!IsInvoking("ShootBlank")) {
        //        InvokeRepeating("ShootBlank", 0, .3f);
        //    }
        //} else {
        //    if (IsInvoking("ShootBlank")) {
        //        CancelInvoke("ShootBlank");
        //    }
        //}
    }


    public GameObject FindClosestEnemy() {
        // Find closest enemy
        var closest = wavesManager.enemiesAlive[0];
        var closestDist = (transform.position - closest.transform.position).sqrMagnitude;

        foreach (var enemy in wavesManager.enemiesAlive) {
            var dist = (transform.position - enemy.transform.position).sqrMagnitude;
            if (dist < closestDist) {
                closest = enemy;
                closestDist = dist;
            }
        }

        return closest;
    }


    public bool AimCannonAtTarget(Vector3 target) {
        Vector3 direction = target - cannonBallSpawnPoint.position;
        float yOffset = direction.y;
        direction = Math3d.ProjectVectorOnPlane(Vector3.up, direction);
        float distance = direction.magnitude;

        float angle0, angle1;
        bool targetInRange = CalculateLaunchAngle(shootingForce, distance, yOffset, Physics.gravity.magnitude, out angle0, out angle1);
        if (!targetInRange)
            return false;

        SetCannonRotation(direction, angle1 * Mathf.Rad2Deg);
        return true;
    }

    bool CalculateLaunchAngle(float speed, float distance, float yOffset, float gravity, out float angle0, out float angle1) {
        angle0 = angle1 = 0;

        float speedSquared = speed * speed;

        float operandA = Mathf.Pow(speed, 4);
        float operandB = gravity * (gravity * (distance * distance) + (2 * yOffset * speedSquared));

        // Target is not in range
        if (operandB > operandA)
            return false;

        float root = Mathf.Sqrt(operandA - operandB);

        angle0 = Mathf.Atan((speedSquared + root) / (gravity * distance));
        angle1 = Mathf.Atan((speedSquared - root) / (gravity * distance));

        return true;
    }

    private void SetCannonRotation(Vector3 planarDirection, float turretAngle) {
        transform.rotation = Quaternion.LookRotation(planarDirection) * Quaternion.Euler(0, 0, 0);
        barrel.localRotation = Quaternion.Euler(0, 0, 0) * Quaternion.AngleAxis(turretAngle, Vector3.left);
    }




    void ShootBlank() {
        GameObject ballObj = Instantiate(cannonBallPrefab, cannonBallSpawnPoint.position, cannonBallSpawnPoint.rotation);
        CannonBall cannonBall = ballObj.GetComponent<CannonBall>();
        cannonBall.shootingForce = shootingForce;

        ballObj.GetComponent<Collider>().enabled = false;
        ballObj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        ballObj.GetComponent<MeshRenderer>().sharedMaterial = blankMat;
    }

    void Shoot() {
        // Set reload cooldown
        shootingTimer = shootingDelay;

        // Instantiate cannon ball and set its force
        GameObject ballObj = Instantiate(cannonBallPrefab, cannonBallSpawnPoint.position, cannonBallSpawnPoint.rotation);
        CannonBall cannonBall = ballObj.GetComponent<CannonBall>();
        cannonBall.shootingForce = shootingForce;

        // Play vfx
        cannonVFX.Play();
        cannonVFX.playRate = 1.2f;
        Sequence lightSequence = DOTween.Sequence();
        lightSequence.Append(
            cannonLight.DOIntensity(50, .5f)
            .SetEase(Ease.OutExpo)
            );
        lightSequence.Append(
            cannonLight.DOIntensity(0, .5f)
            .SetEase(Ease.OutExpo)
            );
        lightSequence.Play();
    }
}
