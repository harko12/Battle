using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerScoreList : MonoBehaviour {

    public GameObject PlayerScoreRowPrefab;

    List<GameObject> scoreRows = new List<GameObject>();

    public ScoreManager sManager;
	// Use this for initialization
	void Start () {

	}
	
    void SetScoreText(GameObject go, string playerName, string scoreType)
    {
        go.transform.Find(scoreType).GetComponent<Text>().text = sManager.GetScore(playerName, scoreType).ToString();
    }

    private int changeCounter = -1;
	// Update is called once per frame
	void Update () {
        if (changeCounter == sManager.ChangeCounter)
        {
            return;
        }

        changeCounter = sManager.ChangeCounter; // update our stored change counter

        var names = sManager.GetPlayerNames();
        foreach (var s in scoreRows)
        {
            s.SetActive(false);
        }

        int lcv = 0;
        foreach (var name in names)
        {
            GameObject go = null;
            if (scoreRows.Count <= lcv)
            {
                go = GameObject.Instantiate(PlayerScoreRowPrefab) as GameObject;
                go.transform.SetParent(this.transform);
                scoreRows.Add(go);
            }
            go = scoreRows[lcv];
            go.SetActive(true);
            go.transform.Find("Username").GetComponent<Text>().text = name;
            SetScoreText(go, name, "Kills");
            SetScoreText(go, name, "Deaths");
            SetScoreText(go, name, "Assists");
            lcv++;
        }
	
	}
}
