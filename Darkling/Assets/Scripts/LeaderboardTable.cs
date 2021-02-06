using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardTable : MonoBehaviour
{

    // populates the leaderboard table with leaderboard rows and fills in stats on those rows

    public GameObject LeaderboardRowPrefab;
    public Vector3 offset = new Vector3(20, 0, 0);

    private void Start()
    {
        CreateRows();
    }


    public void CreateRows()
    {
        DestroyRows();
        Dreamlo.Instance.DownloadAll();

        var userDataList = Dreamlo.Instance.userDataList;

        for (int i = 0; i < userDataList.Count; i++)
        {

            // Instantiate Prefab within the table
            var NewRowObject = Instantiate(LeaderboardRowPrefab, transform.position + offset, Quaternion.identity, transform);

            // Get component
            var newRow = NewRowObject.GetComponent<LeaderboardRow>();

            // Assign variables from userData to text
            newRow.userNameText.text = userDataList[i].userName;
            newRow.bestWaveText.text = userDataList[i].bestWave.ToString();
            newRow.bestKillsText.text = userDataList[i].bestKills.ToString();
            newRow.rankText.text = (i + 1).ToString();



        }

        //foreach (var userData in userDataList)
        //{

        //    // Instantiate Prefab within the table
        //    var NewRowObject = Instantiate(LeaderboardRowPrefab, transform.position + offset, Quaternion.identity, transform);

        //    // Get component
        //    var newRow = NewRowObject.GetComponent<LeaderboardRow>();

        //    // Assign variables from userData to text
        //    newRow.userNameText.text = userData.userName;
        //    newRow.bestWaveText.text = userData.bestWave.ToString();
        //    newRow.bestKillsText.text = userData.bestKills.ToString();
        //    // TODO: newRow.rankText = 

        //    // Put in correct order according to rank

        //}


    }


    public void DestroyRows()
    {
        var allRows = GetComponentsInChildren<LeaderboardRow>();
        foreach (var row in allRows)
            Destroy(row.gameObject);
    }


}
