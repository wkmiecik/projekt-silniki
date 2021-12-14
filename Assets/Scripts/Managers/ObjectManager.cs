using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    // Player objects
    public PlayerMovement player;

    // Ship objects
    public MainShip ship;

    // Ocean manager
    public OceanManager oceanManager;

    // Sun
    public GameObject sun;

    // Day/Night cycle
    public DayNightCycle dayNightCycle;

    // Wave spawner
    public WaveSpawner waveSpawner;

    // UI manager
    public UIManager uiManager;
}
