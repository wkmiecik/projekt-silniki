using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Access to ship
    MainShip ship;

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

        // Rigidbody
        rb = gameObject.GetComponent<Rigidbody>();

        // Animations
        animator = gameObject.GetComponentInChildren<Animator>();

        // Set starting attack timer
        attackDelayTimer = attackDelay;
    }


    void FixedUpdate() {
        // Get closest point on ship collision box
        closestPointOnShip = ship.outsideCollider.ClosestPoint(transform.position);

        // Look towards and go to closest point on ship
        var lookHere = new Vector3(closestPointOnShip.x, transform.position.y, closestPointOnShip.z);
        transform.LookAt(lookHere);
        Vector3 force = Vector3.forward * speed * 10000;
        rb.AddRelativeForce(force * Time.fixedDeltaTime, ForceMode.Force);

        // Slow down if in slow field
        if (slowed) {
            force = Vector3.back * slowSpeed * 10000;
            rb.AddRelativeForce(force * Time.fixedDeltaTime, ForceMode.Force);
        }

        // Destroy if dead
        if (hp <= 0) {
            Destroy(gameObject);
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
