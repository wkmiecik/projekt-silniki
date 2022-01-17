using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Equipment levels strings", menuName = "ScriptableObjects/EquipmentLevelsStrings", order = 1)]
public class EquipmentLevels : ScriptableObject {
    public EquipmentUpgrade[] cannonUpgrades;
    public EquipmentUpgrade[] cannonBallsUpgrades;
    public EquipmentUpgrade[] shipUpgrades;

    public EquipmentUpgrade[] GetUpgrades(ShipEquipment.EquipmentType equipmentType) {
        switch (equipmentType) {
            case (ShipEquipment.EquipmentType.cannon):
                return cannonUpgrades;
            case (ShipEquipment.EquipmentType.cannonBalls):
                return cannonBallsUpgrades;
            case (ShipEquipment.EquipmentType.ship):
                return shipUpgrades;
        }

        throw new UnityException("wrong equipment type");
    }
}
