using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType {
    coin,
    wood,
    iron
}

public class ResourcesManager : MonoBehaviour
{
    // UI manager
    UIManager uiManager;

    [SerializeField] int startingCoins = 0;
    [SerializeField] int startingWood = 0;
    [SerializeField] int startingIron = 0;

    void Start() {
        uiManager = ObjectManager.Instance.uiManager;
        coins = startingCoins;
        wood = startingWood;
        iron = startingIron;
    }


    public int GetResource(ResourceType resourceType) {
        switch (resourceType) {
            case (ResourceType.coin):
                return coins;

            case (ResourceType.wood):
                return wood;

            case (ResourceType.iron):
                return iron;
        }

        throw new UnityException("wrong resource type");
    }

    public int SubtractResource(ResourceType resourceType, int amount) {
        switch (resourceType) {
            case (ResourceType.coin):
                return coins -= amount;

            case (ResourceType.wood):
                return wood -= amount;

            case (ResourceType.iron):
                return iron -= amount;
        }
        throw new UnityException("wrong resource type");
    }
    public int AddResource(ResourceType resourceType, int amount) {
        switch (resourceType) {
            case (ResourceType.coin):
                return coins += amount;

            case (ResourceType.wood):
                return wood += amount;

            case (ResourceType.iron):
                return iron += amount;
        }
        throw new UnityException("wrong resource type");
    }


    // Coins
    private int _coins = 0;
    public int coins {
        get => _coins;
        set {
            if (value < 0) {
                _coins = 0;
                uiManager.SetCoinsText(_coins);
            } else {
                _coins = value;
                uiManager.SetCoinsText(_coins);
            }
        }
    }


    // Wood
    private int _wood = 0;
    public int wood {
        get => _wood;
        set {
            if (value < 0) {
                _wood = 0;
                uiManager.SetWoodText(_wood);
            }
            else {
                _wood = value;
                uiManager.SetWoodText(_wood);
            }
        }
    }


    // Iron
    private int _iron = 0;
    public int iron {
        get => _iron;
        set {
            if (value < 0) {
                _iron = 0;
                uiManager.SetIronText(_iron);
            }
            else {
                _iron = value;
                uiManager.SetIronText(_iron);
            }
        }
    }
}
