using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    // Camera controller
    public CameraController cameraController;

    // Player objects
    public Player player;

    // Ship objects
    public MainShip ship;
    public ShipEquipment shipEquipment;

    // Ocean manager
    public OceanManager oceanManager;

    // Resources manager
    public ResourcesManager resourcesManager;

    // Sun
    public GameObject sun;

    // Day/Night cycle
    public DayNightCycle dayNightCycle;

    // Wave spawner
    public WavesManager wavesManager;

    // UI manager
    public UIManager uiManager;

    // Equipment levels
    public EquipmentLevels equipmentLevels;
}
