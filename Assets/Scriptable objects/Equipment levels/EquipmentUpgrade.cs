using UnityEngine;

[System.Serializable]
public class EquipmentUpgrade
{
    public ResourceType cost_1_resource;
    public Sprite cost_1_icon;
    public int cost_1_amount;

    public ResourceType cost_2_resource;
    public Sprite cost_2_icon;
    public int cost_2_amount;

    public string bonus_1;
    public string bonus_2;
}
