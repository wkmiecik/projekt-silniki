using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Equipment levels strings", menuName = "ScriptableObjects/EquipmentLevelsStrings", order = 1)]
public class EquipmentLevels : ScriptableObject {
    public EquipmentUpgrade[] cannonUpgrades;
    public EquipmentUpgrade[] cannonBallsUpgrades;
    public EquipmentUpgrade[] shipUpgrades;

    public EquipmentUpgrade[] GetUpgrades(EquipmentType equipmentType) {
        switch (equipmentType) {
            case (EquipmentType.cannon):
                return cannonUpgrades;
            case (EquipmentType.cannonBalls):
                return cannonBallsUpgrades;
            case (EquipmentType.ship):
                return shipUpgrades;
        }

        throw new UnityException("wrong equipment type");
    }
}
