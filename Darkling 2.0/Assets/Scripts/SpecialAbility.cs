using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAbility : MonoBehaviour
{



    public int healthCost;
    public float useRate = 2f;
    //public int damage;
    float nextUse;

    void Update()
    {
     
        //if (Input.GetKeyDown(InputManager.Instance.ability1) && Time.time > nextUse)
        //{
        //    if (Stats.Instance.currentHP >= healthCost + 1)
        //    {
        //        Use();
        //    }
        //    else
        //    {
        //        StartCoroutine(HUD.Instance.ShowMessage("Not Enough Life", Color.red, 36, 2f));
        //        return;
        //    }
        //}
        

    }

    void Use()
    {
        nextUse = Time.time + useRate;
        Stats.Instance.currentHP -= healthCost;
        print("Special Ability used");



    }
}
