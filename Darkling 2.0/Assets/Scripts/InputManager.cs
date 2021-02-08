using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

    }

    #endregion

    public KeyCode jump;
    public KeyCode fire, 
        reload, 
        menu, 
        zoom, 
        melee, 
        run, 
        interact, 
        grenade, 
        crouch, 
        grapple,
        changeCamera,
        useSkill,
        selectWeapon0,
        selectWeapon1;

    public bool aimButtonPressed, aimButtonHeld, aimButtonReleased;
    public bool inputSuspended = false;

    /*
    private float chargeCounter;
    public bool ButtonHoldCheck(KeyCode button, float chargeDuration)
    {
        if (Input.GetKey(button))
        {
            chargeCounter += Time.deltaTime;

            if (chargeCounter < chargeDuration)
            {
                // Charging...
                return false;
            }
            else
            {
                // Charged
                return true;
            }
        }

        if (Input.GetKeyUp(button))
        {
            // Charged attack
          //  if (chargeCounter >= chargeDuration)
          //  {
          //
          //  }

            // Button released
            chargeCounter = 0;

        }

        return false;
    }
    */

    private void Update()
    {
        aimButtonPressed = Input.GetKeyDown(zoom);
        aimButtonReleased = Input.GetKeyUp(zoom);
        aimButtonHeld = Input.GetKey(zoom);
    }

   

}
