using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using DG.Tweening;

public class Cannon : MonoBehaviour
{
    public bool active = false;

    [SerializeField]
    GameObject cannonBallPrefab;

    Transform barrel;
    Transform cannonBallSpawnPoint;

    [SerializeField]
    float shootingForce = 500f;

    [SerializeField]
    float shootingDelay = .1f;
    float shootingTimer = 0;

    VisualEffect cannonVFX;
    Light cannonLight;

    private void Start() {
        cannonVFX = GetComponentInChildren<VisualEffect>();
        cannonLight = GetComponentInChildren<Light>();

        barrel = transform.Find("Barrel");
        cannonBallSpawnPoint = barrel.Find("SpawnPoint");
    }

    void Update()
    {
        // Shooting reload timer count
        shootingTimer -= Time.deltaTime;
        if (shootingTimer < -10) shootingTimer = 0;

        if (active) {
            // Wait for click and shoot if reloaded
            if (Input.GetKey(KeyCode.Mouse0) && shootingTimer <= 0) {
                Shoot();
            }
        }
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
