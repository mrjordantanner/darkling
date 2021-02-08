using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour
{
    #region Singleton
    public static Achievements Instance;

    private void Awake()
    {
        if (Application.isEditor)
            Instance = this;
        else
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

    }

    #endregion

    //ACCURACY - "DeadEye"
    // complete 3 waves with 70% accuracy or better
    // complete 5 waves with 80% accuracy or better

    //UPGRADES
    // purchase all upgrades
    // spend 1000 life in one user file  (would grenades count?)
    // Spend your Max Life all the way down to the minimum (5) - "Masochist"

    //KILLS - "Murderous"
    // kill 1000 enemies in one run
    // kill 10,000 enemies in one user file
    // something about exceeding the wave kill quota (x2), (x3)
    // meet the kill quota before time expires for 7 consecutive waves, etc

    //SPECIAL
    // complete 9 waves without ever using grenades
    // complete 6 waves without purchasing any upgrades
    // kill 50 enemies while airborne
    // avoid taking enemy damage for 
    //

    //COLLLECTION - "Health Freak"
    // collect 250 red orbs in one run
    //

}