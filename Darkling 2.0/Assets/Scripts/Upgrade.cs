using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upgrade : MonoBehaviour
{
    // Describes each upgrade, its cost, what it does
    // Sits on the button object 

    public UpgradeController.UpgradeItem upgradeItem;
    public UpgradeTier upgradeTier;
    public string upgradeName;
    public string description;
    public int lifeCost;
    public bool available;

    [HideInInspector]
    public Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public Upgrade(UpgradeTier _upgradeTier, string _upgradeName, string _description, int _lifeCost)
    {
        upgradeTier = _upgradeTier;
        upgradeName = _upgradeName;
        description = _description;
        lifeCost = _lifeCost;


    }
    


}
