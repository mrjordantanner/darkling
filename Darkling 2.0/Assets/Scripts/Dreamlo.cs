using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Dreamlo : MonoBehaviour
{
    #region Singleton
    public static Dreamlo Instance;

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

    // DARKLING DREAMLO

    //public Text leaderBoardText;
    const string privateCode = "RwngFz4N7k2Nm_DLu7_VGwYMmlCMj1s0-0ygfs1ol2zA";
    const string publicCode = "5cd1f54f3ebb3918702db5f6";
    const string webURL = "https://www.dreamlo.com/lb/";
    public List<UserData> userDataList = new List<UserData>();
    public List<string> allUserNames = new List<string>();

    public void UploadData(User user)
    {
        StartCoroutine(UploadNewData(user));
    }

    IEnumerator UploadNewData(User user)
    {
       // DeleteUser(user.userName);  // erase old entry?

        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(user.userName) + "/" + user.userData.bestWave + "/" + user.userData.bestKills);
        // UnityWebRequest www = UnityWebRequest.Post(webURL + privateCode + "/add/" + WWW.EscapeURL(username) + "/" + score);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
            print("Dreamlo: Upload Successful for " + user.userName);
        else
        {
            print("Error uploading: " + www.error);
        }
    }

    public void DownloadAll()
    {
        StartCoroutine(DownloadAllData());   
    }


    IEnumerator DownloadAllData()
    {
        WWW www = new WWW(webURL + publicCode + "/pipe/");
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            allUserNames.Clear();   // clear lists when refreshing all data
            userDataList.Clear();  
            print("Dreamlo:  All usernames downloaded.");
            FormatData(www.text);
        }
        else
        {
            print("Error Downloading: " + www.error);
        }
    }



    // 3)
    public void DownloadUser(string userName)
    {
        if (!allUserNames.Contains(userName))// && !SaveAndLoad.Instance.localUserNames.Contains(userName))
        {   //  if user is already in our list, do nothing
            StartCoroutine(DownloadUserData(userName));
        }
        else print("Dreamlo attempted to download " + userName + " but it's already in allUserNames List");
       
    }
    IEnumerator DownloadUserData(string userName)
    {
        // UnityWebRequest www = UnityWebRequest.Get(webURL + publicCode + "/pipe/");
        WWW www = new WWW(webURL + publicCode + "/pipe-get/" + userName);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            print("Dreamlo: Download Successful for " + userName);
            FormatData(www.text);
        }
        else
        {
            print("Error Downloading: " + www.error);
        }
    }


    public void DeleteUser(string userName)
    {
        WWW www = new WWW(webURL + privateCode + "/delete/" + userName);
        allUserNames.Remove(userName);
        print(userName + " DELETED FROM CLOUD!");
        DownloadAll();  //refresh leaderboards
    }


    //4)
    // Creates a UserData from the downloaded textstream
    void FormatData(string textStream)
    {
        // Count how many entries
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        //print("FormatText Entries: " + entries.Length);
        // Create userData array with that size
        //userDataList = new UserData[entries.Length];

        // Parse though entries and create UserData's from each one
        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });

            string _username = entryInfo[0];
            int _bestWave = int.Parse(entryInfo[1]);
            float _bestKills = float.Parse(entryInfo[2]);

          //  userDataList[i] = new UserData(_username, _wave, _timeRemaining);

            userDataList.Add(new UserData(_username, 0, 0, _bestWave, _bestKills));

            allUserNames.Add(_username);

            //TODO: Update Leaderboard UI
            // print("User Data List: " + userDataList[i].userName + "- Best Wave: " + userDataList[i].bestWave + " / Time Remaining: " + userDataList[i].bestKills);
            
        }
    }

    public bool UsernameUnique(string username)
    {
        if (allUserNames.Contains(username))
        {
            return false;
        }
        else
            return true;

    }



}








