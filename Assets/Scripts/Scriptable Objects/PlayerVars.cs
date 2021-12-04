using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PlayerVars")]
public class PlayerVars : ScriptableObject, ISerializationCallbackReceiver {
    // Movement variables
    public enum MovementMode {
        swimming,
        walking,
        cannonShooting
    }

    [Header("Movement Settings")]
    public MovementMode currentMovementModeSetting = MovementMode.swimming;
    public float normalResistanceForceSetting = 20f;
    public float boostResistanceForceSetting = 7f;
    public float boostLengthSetting = 10f;
    public float boostRecoveryDelaySetting = 2f;


    // Runtime variables
    [HideInInspector] public MovementMode currentMovementMode = MovementMode.swimming;
    [HideInInspector] public float normalResistanceForce;
    [HideInInspector] public float boostResistanceForce;
    [HideInInspector] public float boostLength;
    [HideInInspector] public float boostRecoveryDelay;

    public void OnAfterDeserialize() {
        currentMovementMode = currentMovementModeSetting;
        normalResistanceForce = normalResistanceForceSetting;
        boostResistanceForce = boostResistanceForceSetting;
        boostLength = boostLengthSetting;
        boostRecoveryDelay = boostRecoveryDelaySetting;
    }
    public void OnBeforeSerialize() { }
}
