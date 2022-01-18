using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableResource : MonoBehaviour
{
    // Resources manager
    ResourcesManager resourcesManager;


    private void Start() {
        resourcesManager = ObjectManager.Instance.resourcesManager;
    }


    // Resources to pick up
    [Header("Coins")]
    public bool pickUpCoins;
    public int minimumCoinsAmount = 1;
    public int maximumCoinsAmount = 10;

    [Header("Wood")]
    public bool pickUpWood;
    public int minimumWoodAmount = 1;
    public int maximumWoodAmount = 10;

    [Header("Iron")]
    public bool pickUpIron;
    public int minimumIronAmount = 1;
    public int maximumIronAmount = 10;

    [Header("Destroy Options")]
    public bool destroyOnCollision = true;
    public float destroyDelay = .3f;


    // Was it already picked
    bool alreadyPicked = false;

    private void OnCollisionEnter(Collision collision) {
        if (collision.body.tag == "Player" && !alreadyPicked) {
            alreadyPicked = true;

            if (pickUpCoins) {
                resourcesManager.coins += Random.Range(minimumCoinsAmount, maximumCoinsAmount + 1);
            }

            if (pickUpWood) {
                resourcesManager.wood += Random.Range(minimumWoodAmount, maximumWoodAmount + 1);
            }

            if (pickUpIron) {
                resourcesManager.iron += Random.Range(minimumIronAmount, maximumIronAmount + 1);
            }

            if (destroyOnCollision) {
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
