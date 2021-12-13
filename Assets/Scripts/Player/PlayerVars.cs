using UnityEngine;

public class PlayerVars : Singleton<PlayerVars> {
    // Player variables
    public enum MovementMode {
        swimming,
        walkingOnShip,
        cannonShooting
    }

    public MovementMode currentMovementMode = MovementMode.swimming;

    public float resistanceForce = 0f;
    public float normalResistanceForce = 20f;
    public float boostResistanceForce = 7f;

    public float boostLength = 10f;
    public float boostRecoveryDelay = 2f;

    public float rotationLimitFactor = 2000;

    [HideInInspector] public GameObject usedCannon;
    [HideInInspector] public Vector3 mouseWorldPosition;
}

