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

    // Access to ship equipment
    ShipEquipment shipEquipment;

    public bool playerSteering = false;
    public bool aiSteering = false;

    [SerializeField] GameObject cannonBallPrefab;

    Transform barrel;
    Transform cannonBallSpawnPoint;

    [SerializeField] float shootingForce = 30f;
    [SerializeField] float predictionFactor = 30f;

    float shootingDelayTimer = 0;

    VisualEffect cannonVFX;
    Light cannonLight;

    //[SerializeField] CannonballArc trajectory;

    [Header("Aim marker")]
    [SerializeField] GameObject aimMarkerObject;
    [SerializeField] MeshRenderer aimMarkerRenderer;
    [SerializeField] Color aimMarkerInRangeColor;
    [SerializeField] Color aimMarkerOutOfRangeColor;

    [Header("Angle limits")]
    [SerializeField] float minVerticalAngle = -5;
    [SerializeField] float maxVerticalAngle = 15;
    [SerializeField] float HorizontalRange = 45;
    Quaternion startingRotation;

    private void Start() {
        // Access to Waves Manager
        wavesManager = ObjectManager.Instance.wavesManager;

        // Access to player
        player = ObjectManager.Instance.player;

        // Access to ocean manager
        oceanManager = ObjectManager.Instance.oceanManager;

        // Access to ship equipment
        shipEquipment = ObjectManager.Instance.shipEquipment;


        // Get vfx components
        cannonVFX = GetComponentInChildren<VisualEffect>();
        cannonLight = GetComponentInChildren<Light>();

        // Get barrel and cannonball spawnpoint
        barrel = transform.Find("Barrel");
        cannonBallSpawnPoint = barrel.Find("Cannonball Spawn Point");

        // Set reload timer
        shootingDelayTimer = shipEquipment.cannonReload;

        // Get starting horizontal rotation
        startingRotation = transform.rotation;
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
                aimMarkerObject.transform.position = target;
                aimMarkerRenderer.sharedMaterial.color = aimMarkerInRangeColor;
                aimMarkerObject.SetActive(true);
            }
            else {
                // Set marker to diffrent color if out of range
                aimMarkerObject.transform.position = target;
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

        var barrelAngle = angle1 * Mathf.Rad2Deg;
        var baseAngle = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 0, 0);

        
        // Do nothing if target out of cannon range
        if (!targetInRange) {
            return false;
        }

        // Dont rotate base if out of horizontal range
        var currentRot = baseAngle.eulerAngles;
        var diff = Mathf.Abs(currentRot.y - startingRotation.eulerAngles.y);
        if (diff > HorizontalRange && diff < 360 - HorizontalRange) {
            return false;
        }
        transform.localRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);

        // Dont rotate barrel if out of vertical range
        if (barrelAngle < minVerticalAngle || barrelAngle > maxVerticalAngle) {
            return false;
        }
        barrel.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y - 90, 0) * Quaternion.AngleAxis(barrelAngle, Vector3.left);


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
        shootingDelayTimer = shipEquipment.cannonReload;

        // Instantiate cannon ball, set its force and statistics
        GameObject ballObj = Instantiate(cannonBallPrefab, cannonBallSpawnPoint.position, cannonBallSpawnPoint.rotation);
        CannonBall cannonBall = ballObj.GetComponent<CannonBall>();
        cannonBall.shootingForce = shootingForce;
        cannonBall.damage = shipEquipment.cannonDamage;
        cannonBall.konckback = shipEquipment.cannonKnockback;

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
