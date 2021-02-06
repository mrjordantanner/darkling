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
    public KeyCode fire, menu, zoom, shake, ability1, ability2;

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

    }

   

}
