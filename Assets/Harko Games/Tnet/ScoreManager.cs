using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour {

    Dictionary<string, Dictionary<string, int>> playerScores;
    List<string> scoreFields = new List<string>();

    int mChangeCounter = 0;
    public int ChangeCounter { get { return mChangeCounter; } }
	// Use this for initialization
	void Start () {
        Init();
        SetScore("charlie", "Kills", 0);
        SetScore("charlie", "Deaths", 4);
        SetScore("charlie", "Assists", 1);

        SetScore("Sabrina", "Kills", 10);

        SetScore("Viv", "Kills", 15);

    }
    public void DEBUG_update_Score()
    {
        ChangeScore("charlie", "Kills", 1);
    }
    void Init()
    {
        if (playerScores != null)
            return;

        playerScores = new Dictionary<string, Dictionary<string, int>>();
    }

    public int GetScore(string playerName, string scoreType)
    {
        var ps = GetPlayerScoresForPlayer(playerName);
        InitScoreTypeForPlayer(ps, scoreType);
        return ps[scoreType];
    }

    public void SetScore(string playerName, string scoreType, int value)
    {
        var ps = GetPlayerScoresForPlayer(playerName);
        InitScoreTypeForPlayer(ps, scoreType);
        ps[scoreType] = value;
        mChangeCounter++;
    }

    public void ChangeScore(string playerName, string scoreType, int change)
    {
        var ps = GetPlayerScoresForPlayer(playerName);
        InitScoreTypeForPlayer(ps, scoreType);
        ps[scoreType] += change;
        mChangeCounter++;

    }

    public Dictionary<string, int> GetPlayerScoresForPlayer(string playerName)
    {
        Init();
        if (!playerScores.ContainsKey(playerName))
        {
            playerScores.Add(playerName, new Dictionary<string, int>());
        }
        return playerScores[playerName];
    }

    public void InitScoreTypeForPlayer(Dictionary<string, int> scoresForPlayer, string scoreType)
    {
        if (!scoresForPlayer.ContainsKey(scoreType))
        {
            scoresForPlayer.Add(scoreType, 0);
        }
        return;
    }

    public string[] GetPlayerNames()
    {
        Init();
        return playerScores.Keys.OrderByDescending(n => GetScore(n, "Kills")).ToArray() ;
    }
}