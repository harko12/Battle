using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TNet;

public class LobbyMenu : MonoBehaviour {

    public GameObject TeamPanel, TeamListPanel, TeamRow, PlayerRow;
    public int TeamCount;

    void OnEnable()
    {
        StartCoroutine(UpdatePlayerList());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public PlayerListModel GetPlayerListModel(TNet.Player p)
    {
        return new PlayerListModel()
        {
            Name = p.name,
            TeamID = p.dataNode.GetChild<int>(NetworkManager.NODE_TeamId),
            Kills = p.dataNode.GetChild<int>(NetworkManager.NODE_Kills),
            Deaths = p.dataNode.GetChild<int>(NetworkManager.NODE_Deaths)
        };

    }
    public float refreshTime = .5f;

    List<GameObject> TeamLines = new List<GameObject>();
    List<GameObject> PlayerLines = new List<GameObject>();

    IEnumerator UpdatePlayerList()
    {
        while (true)
        {
            var playerTeam = TNManager.player.dataNode.GetChild<int>(NetworkManager.NODE_TeamId);

            var players = new TNet.List<PlayerListModel>();
            foreach (var p in TNManager.players)
            {
                players.Add(GetPlayerListModel(p));
            }
            players.Add(GetPlayerListModel(TNManager.player));

            int lastTeamId = -2;
            int teamLcv = 0;
            int playerLcv = 0;
            foreach (var pr in players.ToArray()
                .OrderBy(p => p.TeamID)
                .ThenByDescending(p => p.Kills)
                .ThenBy(p => p.Name))
            {
                if (lastTeamId != pr.TeamID)
                {
                    lastTeamId = pr.TeamID;
                    GameObject teamRow = null;
                    if (TeamLines.Count <= teamLcv)
                    {
                        teamRow = GameObject.Instantiate(TeamRow) as GameObject;
                        teamRow.transform.SetParent(TeamPanel.transform);
                        teamRow.SetActive(false);
                        TeamLines.Add(teamRow);
                    }
                    teamRow = TeamLines[teamLcv];
                    teamRow.transform.SetParent(TeamListPanel.transform);
                    teamRow.SetActive(true);
                    var tr = teamRow.GetComponent<TeamRow>();
                    if (playerTeam == -1)
                    {
                        tr.Name.text = "Not yet picked a team.";
                    }
                    else
                    {
                        tr.Name.text = "Team " + lastTeamId.ToString();
                    }
                    teamLcv++;
                }

                GameObject playerRow = null;
                if (PlayerLines.Count <= playerLcv)
                {
                    playerRow = GameObject.Instantiate(PlayerRow) as GameObject;
                    playerRow.transform.SetParent(TeamPanel.transform);
                    playerRow.SetActive(false);
                    PlayerLines.Add(playerRow);
                }
                playerRow = PlayerLines[playerLcv];
                playerRow.transform.SetParent(TeamListPanel.transform);
                playerRow.SetActive(true);
                var prScript = playerRow.GetComponent<PlayerRow>();
                prScript.Name.text = pr.Name;
                prScript.Score.text = string.Format("K: {0} D: {1}", pr.Kills, pr.Deaths);
                prScript.Info.text = "stuff..";
                playerLcv++;
            }
            // hide and move any unused panels until needed
            for (int lcv = teamLcv; lcv < TeamLines.Count; lcv++)
            {
                TeamLines[lcv].transform.SetParent(TeamPanel.transform);
                TeamLines[lcv].SetActive(false);
            }
            for (int lcv = playerLcv; lcv < PlayerLines.Count; lcv++)
            {
                PlayerLines[lcv].transform.SetParent(TeamPanel.transform);
                PlayerLines[lcv].SetActive(false);
            }

            yield return new WaitForSeconds(refreshTime);
        }
    }
}
