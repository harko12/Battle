using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TNet;
using UnityEngine.SceneManagement;

public class PlayerListModel
{
    public string Name {get;set;}
    public int TeamID {get; set;}
    public int Kills {get; set;}
    public int Deaths {get; set;}
}

[System.Serializable]
public class TeamInfo
{
    public int TeamId;
    public string TeamName;
    public Color TeamColor;
    public int PlayerCount;
    public bool Balance;
}

public class NetworkManager : MonoBehaviour
{

    public static NetworkManager instance;
    public GameObject lobbyCamera;
    //public SpawnPoint[] spawnPoints;
    public TeamInfo[] Teams; // teamId is the array index probably should make that better
    public GameObject fps_player;
    public GameObject myCanvas;
    public GameObject crosshair;
    public Text networkMessages;
    public UnityEngine.UI.Text playerName;
    public UnityEngine.UI.Button spawnButton, spButton, mpButton, observerButton;
    public DropDown teamInput;
    public GameObject TeamPanel, MenuPanel;
    public LobbyMenu myLobbyMenu;

    public bool FriendlyFire = true;

    public float RespawnTimer;
    public int PlayersPerTeam;

    private bool PlayerObserving = true;
    private bool mPlayerActive = false;
    private bool PlayerActive
    {
        get
        {
            return mPlayerActive;
        }
        set
        {
            mPlayerActive = value;
            /*
            Screen.lockCursor = mPlayerActive;
            if (FPSPlayer.instance != null)
            {
                FPSPlayer.instance.SetPlayerControlsActive(mPlayerActive);
            }
            */
        }
    }

    void Awake()
    {
        instance = this;
        TNManager.AddRCCs(typeof(NetworkManager));
    }

    // Use this for initialization
    void Start()
    {
        /*
        if (!TNManager.isConnected || !TNManager.isInChannel)
        {
            OnNetworkLeaveChannel();
            return;
        }
         */

        for (int lcv = 0; lcv < Teams.Count(); lcv++)
        {
            Teams[lcv].TeamId = lcv;
        }
        myLobbyMenu = TeamPanel.GetComponent<LobbyMenu>();
        TeamPanel.SetActive(false);
        MenuPanel.SetActive(true);
//        spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();
    }

    public const string NODE_TeamId = "TeamID";
    public const string NODE_Kills = "Kills";
    public const string NODE_Deaths = "Deaths";

    void ToggleLobbyPanel()
    {
        if (!TeamPanel.activeSelf)
        {
            OpenLobbyPanel();
        }
        else
            CloseLobbyPanel();

    }

    void CloseLobbyPanel()
    {
        TeamPanel.SetActive(false);
        PlayerActive = true;
    }

    void OpenLobbyPanel()
    {
        PlayerActive = false;
        if (!TeamPanel.activeSelf)
        {
            TeamPanel.SetActive(true);
        }

        playerName.text = TNManager.playerName;
        var team = TNManager.playerData.GetChild<int>(NODE_TeamId);
        Debug.Log(string.Format("{0} joining channel on team {1}", playerName.text, team.ToString()));
        if (team != -1)
        {
            teamInput.Value = team.ToString();
        }
    }

    void ToggleMenuPanel()
    {
        if (!MenuPanel.activeSelf)
        {
            OpenMenu();
        }
        else
            CloseMenu();

    }

    void OpenMenu()
    {
        PlayerActive = false;
        if (!MenuPanel.activeSelf)
        {
            MenuPanel.SetActive(true);
        }
    }

    void CloseMenu()
    {
        MenuPanel.SetActive(false);
        PlayerActive = true;
    }

    void OnApplicationFocus(bool focusStatus)
    {
        if (PlayerActive && focusStatus)
        {
            /*
            if (FPSPlayer.instance != null)
            {
                FPSPlayer.instance.LockCursor(true);
            }
            */
        }

    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            ToggleLobbyPanel();
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ToggleMenuPanel();
        }

        if (RespawnTimer > 0)
        {
            RespawnTimer -= Time.deltaTime;
            if (RespawnTimer <= 0)
            {
                SpawnPlayer();
            }
        }
    }
    /*
    void OnNetworkConnect(bool success, string message)
    {
        networkMessages.text = message == null ? "Connected" : message;
        TNManager.JoinChannel(1, null, false, 999, null);
    }

    public void Connect()
    {
        networkMessages.text = "Attempting to Connect.";
        TNManager.Connect("localhost");
    }
    */
    public void StartSinglePlayer()
    {
        JoinGame();
    }

    public void JoinGame()
    {
        PlayerObserving = false;
        CloseMenu();
        SpawnPlayer();
    }

    void OnNetworkJoinChannel(int channelID, bool result, string message)
    {
        if (result)
        {
            var p = TNManager.playerData;
            p.SetChild(NODE_TeamId, -1);
            p.SetChild(NODE_Kills, 0);
            p.SetChild(NODE_Deaths, 0);
            //TNManager.SyncPlayerData(); // no longer needed?

            Debug.Log(string.Format("{0} datanode teamid set to {1}", p.name, p.GetChild<int>(NODE_TeamId)));
            OpenLobbyPanel();
            MenuPanel.gameObject.SetActive(false);
            if (TNManager.IsHosting(channelID))
            {
                // update the channel data
                TNManager.channelData = string.Format("{0}'s Game", TNManager.playerName);
            }
        }
        else
        {
            networkMessages.text = "Failed to join: " + message;
        }
    }

    public void LeaveChannel()
    {
        TNManager.LeaveChannel();
    }

    void OnNetworkLeaveChannel()
    {
        SceneManager.LoadScene(0);
    }
    /*
    public SpawnPoint GetRandomSpawnPoint(int teamId)
    {
        var spList = (from SpawnPoint s in spawnPoints
                      where s.teamId == teamId
                      select s).ToList();
        var rand = Random.Range(0, spList.Count);
        return spList[rand];
    }
    */
    public void SpawnPlayer()
    {
        if (teamInput.Value == null || string.IsNullOrEmpty(teamInput.Value.ToString()))
        {
            return;
        }
        /*
        if (FPSPlayer.instance == null)
        {
            int teamId = int.Parse(teamInput.Value.ToString());
            TNManager.playerDataNode.SetChild(NODE_TeamId, teamId);
            UpdatePlayerInfo();
            TNManager.SyncPlayerData();
            var sp = GetRandomSpawnPoint(teamId);
            if (sp != null)
            {
                var spTransform = sp.transform;
                TNManager.CreateEx(10, false, fps_player, spTransform.position, spTransform.rotation, teamId);
            }
        }
        */

        lobbyCamera.SetActive(false);
        TeamPanel.SetActive(false);

        crosshair.SetActive(true);

        PlayerActive = true;
    }

    public void DeSpawnPlayer(bool respawn)
    {
        /*
        if (FPSPlayer.instance != null)
        {
            TNManager.Destroy(FPSPlayer.instance.gameObject);
        }
        */
        PlayerActive = false;
        if (respawn)
        {
            RespawnTimer = 5;
        }


        crosshair.SetActive(false);
        spawnButton.interactable = true;
        lobbyCamera.SetActive(true);
    }

    [RCC(10)]
    public GameObject OnCreatePlayer(GameObject prefab, Vector3 pos, Quaternion rot, int teamId)
    {
        var go = Instantiate(prefab, pos, rot) as GameObject;
        go.name = "_Player_" + TNManager.playerName;
        /*
        var fps = go.GetComponent<FPSPlayer>();
        fps.JoinTeam(teamId);
        */
        return go;
    }

    public void ToggleObserver()
    {
        if (PlayerObserving)
        {
            SpawnPlayer();
            observerButton.gameObject.SetActive(true);
        }
        else
        {
            DeSpawnPlayer(false);
            observerButton.gameObject.SetActive(false);
        }
        PlayerObserving = !PlayerObserving;
    }

    public void UpdatePlayerInfo()
    {
        TNManager.playerData.SetChild(NODE_TeamId, int.Parse(teamInput.Value.ToString()));
    }
}
