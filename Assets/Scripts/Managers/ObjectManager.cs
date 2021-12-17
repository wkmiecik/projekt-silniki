using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    // Player objects
    public Player player;

    // Ship objects
    public MainShip ship;

    // Ocean manager
    public OceanManager oceanManager;

    // Sun
    public GameObject sun;

    // Day/Night cycle
    public DayNightCycle dayNightCycle;

    // Wave spawner
    public WavesManager wavesManager;

    // UI manager
    public UIManager uiManager;
}
