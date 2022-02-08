using UnityEngine;

[System.Serializable]
public class EquipmentUpgrade
{
    [Header("Cost")]
    public ResourceType cost_1_resource;
    public Sprite cost_1_icon;
    public int cost_1_amount;

    public ResourceType cost_2_resource;
    public Sprite cost_2_icon;
    public int cost_2_amount;

    [Header("Bonus")]
    public UpgradeBonusType bonus_1;
    public float bonus_1_amount;
    public UpgradeBonusType bonus_2;
    public float bonus_2_amount;
}
