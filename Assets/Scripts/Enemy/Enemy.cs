using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Access to Ship
    MainShip ship;

    // Access to Waves Manager
    WavesManager wavesManager;

    // Target
    Vector3 closestPointOnShip;

    // Movement
    Rigidbody rb;
    [SerializeField] float speed = 10;
    [SerializeField] float slowSpeed = 5;
    [HideInInspector] public bool slowed = false;

    // HP
    [SerializeField] public int hp = 100;

    // Attack
    [SerializeField] int damage = 5;
    [SerializeField] float attackDistance = .5f;
    [SerializeField] float attackDelay = 10f;
    float attackDelayTimer;

    // Animations
    Animator animator;



    void Start()
    {
        // Access to ship
        ship = ObjectManager.Instance.ship;

        // Access to Waves Manager
        wavesManager = ObjectManager.Instance.wavesManager;

        // Rigidbody
        rb = gameObject.GetComponent<Rigidbody>();

        // Animations
        animator = gameObject.GetComponentInChildren<Animator>();

        // Set starting attack timer
        attackDelayTimer = attackDelay;
    }


    void FixedUpdate() {
        // Look at and go to ship
        var lookHere = new Vector3(ship.transform.position.x, transform.position.y, ship.transform.position.z);
        transform.LookAt(lookHere);

        // Set speed
        Vector3 force = Vector3.forward * speed * 10000;
        // Slow down if in slow field
        if (slowed) {
            force = Vector3.forward * slowSpeed * 10000;
        }

        // Add force
        rb.AddRelativeForce(force * Time.fixedDeltaTime, ForceMode.Force);

        // Destroy if dead
        if (hp <= 0) {
            wavesManager.KillEnemy(gameObject);
        }
    }


    private void Update() {
        // Attack if ready and close enough to ship
        var sqrDist = (closestPointOnShip - transform.position).sqrMagnitude;
        if (attackDelayTimer <= 0 && sqrDist < attackDistance * attackDistance) {
            attackDelayTimer = attackDelay;
            Attack();
        }

        // Update attack delay timer
        attackDelayTimer -= Time.deltaTime;
    }


    void Attack() {
        // Damage ship with delay so it can match attack animation
        StartCoroutine(ApplyDamageAfterDelay(.4f));

        // Play animation
        animator.enabled = true;
        animator.Play("EnemyAttack", -1, 0f);
    }


    IEnumerator ApplyDamageAfterDelay(float delayTime) {
        yield return new WaitForSeconds(delayTime);
        // Damage ship
        ship.getHit(damage);
    }
}
