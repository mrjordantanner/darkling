using UnityEngine;

[System.Serializable]
public class User
{
    //public string userID;  // to be used locally to identify users (range 1-4)
    public string userName;
    public SaveSlot saveSlot;
    public UserData userData;

    
    public User(string _userName, UserData _userData)
    {
        userName = _userName;
        userData = _userData;
     
    }
    
}

[System.Serializable]
public class UserData    // changed from Struct to Class
{
    public string userName;
    public int rank;
    public int currentWave, bestWave;
    public float currentKills, bestKills;

    // public float totalPlayTime;
    // public int totalKills, totalDeaths;

    //public int totalLifeSpent;
    // public float bestAccuracy;

    // TODO: Achievements earned

    public UserData(string _userName, int _currentWave, float _currentKills, int _bestWave, float _bestKills)
    {
        userName = _userName;
        currentWave = _currentWave;
        currentKills = _bestKills;
        bestWave = _bestWave;
        bestKills = _bestKills;

        // TODO: totalKills, totalDeaths, totalPlayTime;
    }

}



