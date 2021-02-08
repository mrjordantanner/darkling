using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UserController : MonoBehaviour
{
    #region Singleton
    public static UserController Instance;

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

    //DARKLING USERCONTROLLER

    // SaveAndLoad.Instance.allLocalUsers - use this as reference to local Users
    public CanvasGroup userPanel;
    public CanvasGroup newUserPanel;
    public CanvasGroup userNameTakenPanel;

    public GameObject UserButtonPrefab;

    public User activeUser;
   // public SaveSlot activeSaveSlot;

    public SaveSlot[] saveSlots;
    public List<int> saveSlotIndexes = new List<int>();
    public int maxUsers = 4;

    void Start()
    {
        StartCoroutine(CreateLocalUsers());

        saveSlots = FindObjectsOfType<SaveSlot>();
        foreach (var slot in saveSlots)
        {
            saveSlotIndexes.Add(slot.index);
        }

        // Create blank user for the activeUser User container
       // activeUser = new User("Placeholder", new UserData("Placeholder", 0, 0, 0, 0));

    }
    

    // 1)
    public IEnumerator CreateLocalUsers()
    {
        SaveAndLoad.Instance.LoadLocalUsers();

        // Search cloud for each username and fetch corresponding UserData 
        // This is why if the name doesnt exist in the cloud, it doesnt get loaded, even if it exists locally
        foreach (var localUser in SaveAndLoad.Instance.localUserNames)
        {
            Dreamlo.Instance.DownloadUser(localUser);
        }

        // Cloud download delay - look into yielding to another coroutine for this instead
        yield return new WaitForSeconds(1f);

       // print("SaveAndLoad.localUserNames.Count: " + SaveAndLoad.Instance.localUserNames.Count);
      //  print("Dreamlo.userDataList.Count: " + Dreamlo.Instance.userDataList.Count);

        // )5
        //  // Create new "Users" from our data for use during play
        foreach (var userData in Dreamlo.Instance.userDataList)
        {
            // Make only local users?
            if (SaveAndLoad.Instance.localUserNames.Contains(userData.userName))
            {
                UserData newUserData = new UserData(userData.userName, 0, 0, userData.bestWave, userData.bestKills);      // retrieved from UserData
                CreateUser(userData.userName, newUserData);
                print("Created & Loaded :" + newUserData.userName + " // " + newUserData.bestWave + " // " + newUserData.bestKills);
            }
        }


    }


    public void SetCurrentStats()
    {

        activeUser.userData.currentWave = WaveController.Instance.currentWave;
        activeUser.userData.currentKills = WaveController.Instance.killsThisWave;

    }


    public void CheckForBest()
    {
        SetCurrentStats();

        if (activeUser.userData.bestWave < activeUser.userData.currentWave)
        {
            activeUser.userData.bestWave = activeUser.userData.currentWave;
            activeUser.userData.bestKills = activeUser.userData.currentKills;
            Stats.Instance.newBestRun = true;
            //Dreamlo.Instance.UploadData(activeUser);
        }
        else if (activeUser.userData.bestWave == activeUser.userData.currentWave)
        {
            
            if (activeUser.userData.currentKills > activeUser.userData.bestKills)
            {
                activeUser.userData.bestWave = activeUser.userData.currentWave;
                activeUser.userData.bestKills = activeUser.userData.currentKills;
                Stats.Instance.newBestRun = true;
               // Dreamlo.Instance.UploadData(activeUser);
            }

        }
        else
            Stats.Instance.newBestRun = false;

    }


    public bool HasReachedMaxUsers()
    {
        SaveAndLoad.Instance.LoadLocalUsers();

        if (SaveAndLoad.Instance.localUserNames.Count == maxUsers)
        {
           // GameManager.Instance.addNewUserButton.interactable = false;
            return true;
        }
        else return false;
           // GameManager.Instance.addNewUserButton.interactable = true;
    }


    // Create New, Empty User
    public void CreateUser(string newUserName)
    {
        User newUser = new User(newUserName, new UserData(newUserName, 0, 0, 0, 0));
        SaveUser(newUser);

    }

    // For re-creating returning users
    public void CreateUser(string newUserName, UserData userData)
    {
        User newUser = new User(newUserName, userData);
        SaveUser(newUser);

    }

    void SaveUser(User newUser)
    {
        FindUnusedSaveSlot(newUser);

        // Set this new user as the active one
        SetActiveUser(newUser);

        // Save UserData locally
        SaveAndLoad.Instance.Save(newUser);

        // Clear input field
        GameManager.Instance.ClearInputText();

        // Add to cloud
        Dreamlo.Instance.UploadData(newUser);

        // Refresh local users
        SaveAndLoad.Instance.LoadLocalUsers();

        GameManager.Instance.CheckAddNewButton();


    }






    void FindUnusedSaveSlot(User newUser)
    {
        foreach (var slot in saveSlots)
        {
            if (slot.index == 1 && !slot.inUse)
            {
                slot.SetButton(newUser);
                newUser.saveSlot = slot;
                break;
            }
            if (slot.index == 2 && !slot.inUse)
            {
                slot.SetButton(newUser);
                newUser.saveSlot = slot;
                break;
            }
            if (slot.index == 3 && !slot.inUse)
            {
                slot.SetButton(newUser);
                newUser.saveSlot = slot;
                break;
            }
            if (slot.index == 4 && !slot.inUse)
            {
                slot.SetButton(newUser);
                newUser.saveSlot = slot;
                break;
            }
            //else
            //{
            //    print("All local save slots in use!");
            //    break;
            //}


        }
    }

    public int GetIndexOfLowestValue(List<int> arr)
    {
        float value = float.PositiveInfinity;
        int index = -1;
        for (int i = 0; i < arr.Count; i++)
        {
            if (arr[i] < value)
            {
                index = i;
                value = arr[i];
            }
        }
        return index;
    }

    public void DeleteActiveUser()
    {
        // Delete cloud username
        Dreamlo.Instance.DeleteUser(activeUser.userName);

        // Delete local user file
        SaveAndLoad.Instance.DeleteLocalUser(activeUser);

        ClearActiveSaveSlot();

       // Destroy(user); 
    }


    public void ClearActiveSaveSlot ()
    {
        activeUser.saveSlot.ClearButton();
    }

    public void SetActiveUser(User user)
    {
       // activeUser = user;
        activeUser.userName = user.userName;
        activeUser.userData = user.userData;
        activeUser.saveSlot = user.saveSlot;
       // print("Active User Set: " + activeUser.userName + activeUser.userData);
    }




    // 1) UserController.CreateLocalUsers()   //
    // ) Dreamlo.DownloadAll();       // Populate allUserNames List and userDataList from cloud
    // ) UserController.CreateUser();   // "create" users 







}
