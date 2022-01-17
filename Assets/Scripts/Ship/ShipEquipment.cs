using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEquipment : MonoBehaviour
{
    // Access to UI manager
    UIManager uiManager;

    // Access to resources manager
    ResourcesManager resourcesManager;

    // Access to equipment levels
    EquipmentLevels equipmentLevels;

    // Levels
    public int cannonLevel = 1;
    public int cannonBallsLevel = 1;
    public int shipLevel = 1;

    // Equipment types
    public enum EquipmentType {
        cannon = 0,
        cannonBalls = 1,
        ship = 2
    }


    private void Start() {
        // Access to UI manager
        uiManager = ObjectManager.Instance.uiManager;

        // Access to resources manager
        resourcesManager = ObjectManager.Instance.resourcesManager;

        // Access to equipment levels
        equipmentLevels = ObjectManager.Instance.equipmentLevels;
    }


    private void OnTriggerStay(Collider other) {
        // Check if player is in trigger
        if (other.gameObject.tag == "Legs") {
            // Open equipment ui if pressed E
            if (Input.GetKey(KeyCode.E)) {
                uiManager.OpenEquipment();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Legs") {
            uiManager.EnablePlayerTipText();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Legs") {
            uiManager.DisablePlayerTipText();
        }
    }


    private void Update() {
        if ( uiManager.scrollOpened && (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.E)) ) {
            uiManager.CloseEquipment();
        }
    }


    // Upgrading equipment returns 1 when successfully upgraded, 0 when not enough resources , -1 when level is max
    public int UpgradeEquipment(EquipmentType equipmentType) {
        int levelIndex = GetEquipmentLevel(equipmentType) - 1;

        if (levelIndex < equipmentLevels.GetUpgrades(equipmentType).Length) {
            EquipmentUpgrade upgrade = equipmentLevels.GetUpgrades(equipmentType)[levelIndex];
            if (upgrade.cost_1_amount <= resourcesManager.GetResource(upgrade.cost_1_resource) && upgrade.cost_2_amount <= resourcesManager.GetResource(upgrade.cost_2_resource)) {
                IncreaseLevel(equipmentType);
                resourcesManager.SubtractResource(upgrade.cost_1_resource, upgrade.cost_1_amount);
                resourcesManager.SubtractResource(upgrade.cost_2_resource, upgrade.cost_2_amount);
                uiManager.ShowHoverInfoPanel((int)equipmentType);
                return 1;
            }
            return 0;
        }
        return -1;
    }



    public int GetEquipmentLevel(EquipmentType equipmentType) {
        switch (equipmentType) {
            case (EquipmentType.cannon):
                return cannonLevel;
            case (EquipmentType.cannonBalls):
                return cannonBallsLevel;
            case (EquipmentType.ship):
                return shipLevel;
        }
        throw new UnityException("wrong equipment type");
    }

    private int IncreaseLevel(EquipmentType equipmentType) {
        switch (equipmentType) {
            case (EquipmentType.cannon):
                cannonLevel += 1;
                uiManager.SetEquipmentLevelText(equipmentType, cannonLevel);
                return cannonLevel;
            case (EquipmentType.cannonBalls):
                cannonBallsLevel += 1;
                uiManager.SetEquipmentLevelText(equipmentType, cannonBallsLevel);
                return cannonBallsLevel;
            case (EquipmentType.ship):
                shipLevel += 1;
                uiManager.SetEquipmentLevelText(equipmentType, shipLevel);
                return shipLevel;
        }
        throw new UnityException("wrong equipment type");
    }
}
