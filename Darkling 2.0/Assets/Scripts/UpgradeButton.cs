using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    Upgrade upgrade;

    private void Start()
    {
        upgrade = GetComponent<Upgrade>();
    }

    //private void OnMouseOver()
    //{
    //    print("button mouseover");
    //    UpgradeController.Instance.upgradeDescriptionField.text = upgrade.description;

    //}

    //private void OnMouseExit()
    //{

    //    UpgradeController.Instance.upgradeDescriptionField.text = "";

    //}

    public void PointerEnter()
    {
        UpgradeController.Instance.upgradeDescriptionField.text = upgrade.description;
    }

    public void PointerExit()
    {
        UpgradeController.Instance.upgradeDescriptionField.text = "";
    }


}
