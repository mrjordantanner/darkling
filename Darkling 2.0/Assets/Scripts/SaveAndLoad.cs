using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SaveAndLoad : MonoBehaviour
{
    #region Singleton
    public static SaveAndLoad Instance;

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

    // BINARY

    public List<string> localUserNames = new List<string>();
    const string folderName = "BinaryUserData";
    const string fileExtension = ".dat";

    // Save user to disk
    public void Save(User user)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string dataPath = Path.Combine(folderPath, user.userName + fileExtension);
        SaveUserBinary(user.userName, dataPath);
    }

    static void SaveUserBinary(string userName, string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.OpenOrCreate))
        {
            binaryFormatter.Serialize(fileStream, userName);
        }
    }

    // 2)
    public void LoadLocalUsers()
    {
        // clear list before refreshing
        localUserNames.Clear();  

        // Get all file paths in target directory
        string[] filePaths = GetFilePaths();
       // print("SaveAndLoad found " + filePaths.Length + " filepaths in /" + folderName);

        // Load the results into localUserNames List
        if (filePaths.Length > 0)
        {
            for (int i = 0; i < filePaths.Length; i++)
            {
                localUserNames.Add(LoadUserBinary(filePaths[i]));
              //  print("Loaded " + localUserNames[i] + " from disk.");
            }

        }
    }

    // Load local user by name
    public void Load(User user)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        string dataPath = Path.Combine(folderPath, user.userName + fileExtension);
        if (File.Exists(dataPath))
            LoadUserBinary(dataPath);
        else print("Unable to load local user file - File not found");
    }

    static string LoadUserBinary(string path)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        using (FileStream fileStream = File.Open(path, FileMode.Open))
        {
            return (string)binaryFormatter.Deserialize(fileStream);
        }
    }

    static string[] GetFilePaths()
    {
         string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        return Directory.GetFiles(folderPath);//, fileExtension);
    }


    public void DeleteLocalUser(User user)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, folderName);
        string dataPath = Path.Combine(folderPath, user.userData.userName + fileExtension);
        if (File.Exists(dataPath))
            File.Delete(dataPath);
        else print("Unable to delete local user file - File not found");
    }






}





