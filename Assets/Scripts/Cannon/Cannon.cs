using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using DG.Tweening;

public class Cannon : MonoBehaviour
{
    // Access to Waves Manager
    WavesManager wavesManager;

    // Access to player
    Player player;

    // Access to ocean manager
    OceanManager oceanManager;

    public bool playerSteering = false;
    public bool aiSteering = false;

    [SerializeField] GameObject cannonBallPrefab;

    Transform barrel;
    Transform cannonBallSpawnPoint;

    [SerializeField] float shootingForce = 30f;
    [SerializeField] float predictionFactor = 30f;

    [SerializeField] float shootingDelay = .1f;
    float shootingDelayTimer = 0;

    VisualEffect cannonVFX;
    Light cannonLight;

    //[SerializeField] CannonballArc trajectory;

    [Header("Aim marker")]
    [SerializeField] GameObject aimMarkerObject;
    [SerializeField] MeshRenderer aimMarkerRenderer;
    [SerializeField] Color aimMarkerInRangeColor;
    [SerializeField] Color aimMarkerOutOfRangeColor;

    private void Start() {
        // Access to Waves Manager
        wavesManager = ObjectManager.Instance.wavesManager;

        // Access to player
        player = ObjectManager.Instance.player;

        // Access to ocean manager
        oceanManager = ObjectManager.Instance.oceanManager;

        // Get vfx components
        cannonVFX = GetComponentInChildren<VisualEffect>();
        cannonLight = GetComponentInChildren<Light>();

        // Get barrel and cannonball spawnpoint
        barrel = transform.Find("Barrel");
        cannonBallSpawnPoint = barrel.Find("Cannonball Spawn Point");

        // Set reload timer
        shootingDelayTimer = shootingDelay * .5f;
    }

    void Update()
    {
        // If player steering cannon
        if (playerSteering) {
            // Reload timer counting
            shootingDelayTimer -= Time.deltaTime;

            // Set aiming target to mouse position
            var mousePos = player.mouseWorldPosition;
            var target = new Vector3(mousePos.x, oceanManager.GetHeightAtPosition(mousePos) + 2.5f, mousePos.z);

            // Aim cannon at target and get predicted flight time
            var inRange = AimCannonAtTarget(target);

            // Wait for click and shoot if reloaded
            if (Input.GetKey(KeyCode.Mouse0) && shootingDelayTimer <= 0) {
                Shoot();
            }

            if (inRange) {
                // Show aim marker and set its position to mouse
                aimMarkerObject.transform.position = mousePos;
                aimMarkerRenderer.sharedMaterial.color = aimMarkerInRangeColor;
                aimMarkerObject.SetActive(true);
            }
            else {
                // Set marker to diffrent color if out of range
                aimMarkerObject.transform.position = mousePos;
                aimMarkerRenderer.sharedMaterial.color = aimMarkerOutOfRangeColor;
            }
        } else {
            // Hide aim marker if not using cannon
            aimMarkerObject.SetActive(false);
        }


        // If ai steering cannon
        if (aiSteering) {
            // Check if any enemies alive
            if (wavesManager.enemiesAlive.Count > 0) {
                // Get closest enemy
                var targetRB = FindClosestEnemy().GetComponent<Rigidbody>();
                var aimPos = new Vector3(targetRB.position.x, targetRB.position.y + 1.5f, targetRB.position.z);

                // Predict where to aim
                var dist = (transform.position - targetRB.position).magnitude;
                var velocity = new Vector3(targetRB.velocity.x, 0, targetRB.velocity.z);
                var predictedTargetPos = aimPos + (velocity * .2f * dist * predictionFactor);

                // Aim cannon at predicted target positon
                var inRange = AimCannonAtTarget(predictedTargetPos);


                // Shoot if enemy in range and reloaded
                if (inRange) {
                    // Reload timer counting
                    shootingDelayTimer -= Time.deltaTime;

                    if (shootingDelayTimer <= 0) {
                        Shoot();
                    }
                }
            }
        }


        // Show/hide trajectory
        //if (aiSteering || playerSteering) {
        //    trajectory.lineRenderer.enabled = true;
        //}
        //else {
        //    trajectory.lineRenderer.enabled = false;
        //}
    }


    public GameObject FindClosestEnemy() {
        // Find and return closest enemy
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
        bool targetInRange = TrajectoryMath.CalculateLaunchAngle(shootingForce, distance, yOffset, -Physics.gravity.y, out angle0, out angle1);

        //trajectory.UpdateArc(shootingForce, distance, -Physics.gravity.y, angle1, direction, targetInRange);

        if (!targetInRange)
            return false;

        var turretAngle = angle1 * Mathf.Rad2Deg;
        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 0, 0);
        barrel.localRotation = Quaternion.Euler(0, 0, 0) * Quaternion.AngleAxis(turretAngle, Vector3.left);

        return true;
    }

    public float PredictFlightTime(Vector3 target) {
        Vector3 direction = target - cannonBallSpawnPoint.position;
        float yOffset = direction.y;
        direction = Math3d.ProjectVectorOnPlane(Vector3.up, direction);
        float distance = direction.magnitude;

        float angle0, angle1;
        bool targetInRange = TrajectoryMath.CalculateLaunchAngle(shootingForce, distance, yOffset, -Physics.gravity.y, out angle0, out angle1);

        if (!targetInRange)
            return -1;

        return TrajectoryMath.TimeOfFlight(shootingForce, angle1, yOffset, -Physics.gravity.y);
    }

    void Shoot() {
        // Set reload cooldown
        shootingDelayTimer = shootingDelay;

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
