using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ControlPanel : MonoBehaviour
{
    // Developer hotkeys, shortcuts, etc

    private void Update()
    {

        // Add active user data to cloud
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            if (UserController.Instance.activeUser != null)
                Dreamlo.Instance.UploadData(UserController.Instance.activeUser);
            print("Added " + UserController.Instance.activeUser + " to cloud.");

        }

        // Get active user data from cloud
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (UserController.Instance.activeUser != null)
                Dreamlo.Instance.DownloadUser(UserController.Instance.activeUser.userName);
            print("Downloaded " + UserController.Instance.activeUser + " data from cloud.");

        }

        // Save active user data (best stats) locally
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SaveAndLoad.Instance.Save(UserController.Instance.activeUser);
            print("Local data saved for " + UserController.Instance.activeUser.userName + ".");
        }

        // Load all local user datas (best stats for all users)
        if (Input.GetKeyDown(KeyCode.Keypad6))
            SaveAndLoad.Instance.LoadLocalUsers();
       



        //// SPAWNING
        //// Spawn Enemies
        //if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    SpawnerController.Instance.PopulateGroups();
        //    SpawnerController.Instance.Spawn();
        //}

        //// Clear Enemies
        //if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    SpawnerController.Instance.ClearGroups();
        //    SpawnerController.Instance.DestroyEnemyChildren();
        //}

        //// WAVES
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    WaveController.Instance.SetupNextWave();
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    WaveController.Instance.EndWave();
        //}

        ////  Increase wave number
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    WaveController.Instance.currentWave++;
        //}

        ////  Decrease wave number
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    WaveController.Instance.currentWave--;
        //}


    }

}
