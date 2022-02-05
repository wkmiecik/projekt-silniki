using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimations : MonoBehaviour
{
    // Get player
    Player player;

    // Animators
    [SerializeField] Animator boatAnimator;
    [SerializeField] Animator legsAnimator;

    // Cannon grab
    [SerializeField] ChainIKConstraint cannonGrabLeft;
    [SerializeField] ChainIKConstraint cannonGrabRight;
    float targetWeight;

    [SerializeField] 

    void Start()
    {
        // Get player
        player = ObjectManager.Instance.player;
    }

    void Update()
    {
        // Reset states
        if (boatAnimator.isActiveAndEnabled) {
            boatAnimator.SetBool("rowL", false);
            boatAnimator.SetBool("rowR", false);
        }
        if (legsAnimator.isActiveAndEnabled) {
            legsAnimator.SetBool("walking", false);
        }


        // Boat animations
        if (player.currentMovementMode == Player.MovementMode.swimming) {
            if (player.rot > .1f || player.rot < -.1f) {
                boatAnimator.SetBool("rowR", true);
                boatAnimator.SetBool("rowL", true);
            }

            if (player.acc > .1f || player.acc < -.1f) {
                boatAnimator.SetBool("rowL", true);
                boatAnimator.SetBool("rowR", true);
            }
        }


        // Walking animations
        if (player.currentMovementMode == Player.MovementMode.walkingOnShip) {
            if (player.acc > .1f || player.acc < -.1f || player.rot > .1f || player.rot < -.1f) {
                legsAnimator.SetBool("walking", true);
            }
        }


        // Shooting cannon
        if (player.currentMovementMode == Player.MovementMode.cannonShooting) {
            targetWeight = 1;
        } else {
            targetWeight = 0;
        }
        // Update weigths
        cannonGrabLeft.weight = Mathf.Lerp(cannonGrabLeft.weight, targetWeight, Time.deltaTime * 5f);
        cannonGrabRight.weight = Mathf.Lerp(cannonGrabRight.weight, targetWeight, Time.deltaTime * 5f);
    }
}
