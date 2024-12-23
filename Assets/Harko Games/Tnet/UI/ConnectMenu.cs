using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TNet;
using System.Net;

public class ConnectMenu : MonoBehaviour {
    public UnityEngine.UI.Button ServerButton, ConnectButton;
    public Text Messages;
    public InputField serverAddress, playerName;
    public string GameID = "Shooter_Tutorial";
    private string serverFileName;
    public GameObject ServerListRoot;
    public GameObject ServerLine;
    public GameObject JoinPanel;

    public PersistentInfo pInfo;
    
    public int serverTcpPort = 5127;
    private string mMessage = "";

    private CanvasGroup connectGroup, joinGroup;

    const string INPUT_SERVERNAME = "servername";

    private void OnEnable()
    {
        TNManager.onConnect += OnNetworkConnect;
    }

    private void OnDisable()
    {
        TNManager.onConnect -= OnNetworkConnect;
    }

    // Use this for initialization
    void Start () {
        serverFileName = GameID + ".dat";
        if (Application.isPlaying)
        {
            // Start resolving IPs
            Tools.ResolveIPs(null);

            // We don't want mobile devices to dim their screen and go to sleep while the app is running
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Make it possible to use UDP using a random port
            TNManager.StartUDP(Random.Range(10000, 50000));

            StartCoroutine("UpdateServerList");

            joinGroup = JoinPanel.GetComponent<CanvasGroup>();
            connectGroup = GetComponent<CanvasGroup>();
#if UNITY_WEBPLAYER
            ServerButton.gameObject.SetActive(false);
#endif
        }
        Init();
	}
	
    void Init()
    {
        var name = TNManager.playerName;
        if (name.ToLower() != "guest")
        {
            playerName.text = TNManager.playerName;
        }
        else
        {
            name = CommandLineArgReader.GetArg("-playerName");
            if (name != null)
            {
                playerName.text = name;
            }
        }

        var serverIP = "68.66.205.126:5137"; // pInfo.ServerIP;
        if (!string.IsNullOrEmpty(serverIP))
        {
            serverAddress.text = serverIP;
        }
    }

	// Update is called once per frame
	void Update () {
        Messages.text = mMessage;
        UpdateMenuToggle();
        UpdateServerButton();
        UpdateConnectButton();

	}

    public void OnConnect()
    {
        IPEndPoint extAddress, intAddress;
        extAddress = intAddress = Tools.ResolveEndPoint(serverAddress.text);
        pInfo.ServerIP = serverAddress.text;
        Connect(extAddress, intAddress);
    }

    void Connect(IPEndPoint extAddress, IPEndPoint intAddress)
    {
        if (string.IsNullOrEmpty(playerName.text))
        {
            mMessage = "Player Name Required.";
            return;
        }
        TNManager.playerName = playerName.text;
        Debug.LogFormat("connecting to ext: {0} int: {1}", extAddress, intAddress);
        TNManager.Connect(extAddress, intAddress);
        mMessage = "Connecting...";
    }

    void OnNetworkConnect(bool success, string error)
    {
        if (!success)
        {
            Debug.Log("connection failed: " + error);
            mMessage = error;
        }
    }

    void OnNetworkDisconnect()
    {
        mMessage = "Disconnected";
    }

    public void OpenServerDialog()
    {
        var inputs = new System.Collections.Generic.List<InputItem>();
        var i = new InputItem(); // ScriptableObject.CreateInstance<InputItem>();
        i.InputName = INPUT_SERVERNAME; i.InputType = typeof(string).Name;
        inputs.Add(i);
        //        inputs.Add(new InputItem() { InputName = INPUT_SERVERNAME, InputType = typeof(string).Name });
        InputDialog.instance.OpenDialog(inputs, delegate { OnStartServer(); });

        //InputDialogManager.instance.OpenDialog("StartServer", inputs);
    }

    void OnStartServer()
    {
#if UNITY_WEBPLAYER
					mMessage = "Can't host from the Web Player due to Unity's security restrictions";
#else
        // Start a local server, loading the saved data if possible
        // The UDP port of the server doesn't matter much as it's optional,
        // and the clients get notified of it via Packet.ResponseSetUDP.
        int udpPort = Random.Range(10000, 40000);
        LobbyClient lobby = GetComponent<LobbyClient>();
        var serverStarted = false;
        if (lobby == null)
        {
            serverStarted = TNServerInstance.Start(serverTcpPort, udpPort, serverFileName);
        }
        else
        {
            TNServerInstance.Type type = (lobby is TNUdpLobbyClient) ?
                TNServerInstance.Type.Udp : TNServerInstance.Type.Tcp;
            serverStarted = TNServerInstance.Start(serverTcpPort, udpPort, serverFileName, type, Tools.ResolveEndPoint(lobby.remoteAddress, lobby.remotePort));
//            TNServerInstance.Start(serverTcpPort, udpPort, lobby.remotePort, serverFileName, type);

            var servername = InputDialog.instance.GetValueString(INPUT_SERVERNAME);
            if (!string.IsNullOrEmpty(servername))
                TNServerInstance.serverName = servername;
        }
        if (serverStarted)
        {
            mMessage = "Server started";

            InputDialog.instance.CloseDialog();
        }
#endif

    }

    void OnStopServer()
    {
        TNServerInstance.Stop();
        mMessage = "Server Stopped.";
    }

    void UpdateServerButton()
    {
#if UNITY_WEBPLAYER
        ServerButton.interactable = false;
#else
        var bgImage = ServerButton.GetComponent<Image>();
        var txt = ServerButton.GetComponentInChildren<Text>();
        if (TNServerInstance.isActive)
        {
            bgImage.color = Color.red;
            txt.text = "Stop Server";
            ServerButton.onClick.RemoveAllListeners();
            ServerButton.onClick.AddListener(delegate { OnStopServer(); });
        }
        else
        {
            bgImage.color = Color.green;
            txt.text = "Start Server";
            ServerButton.onClick.RemoveAllListeners();
            ServerButton.onClick.AddListener(delegate { OpenServerDialog(); });
//            ServerButton.onClick.AddListener(delegate { OnStartServer(); });
        }
#endif
    }

    void UpdateConnectButton()
    {
        var enabled = true;
        if (string.IsNullOrEmpty(playerName.text) || string.IsNullOrEmpty(serverAddress.text))
        {
            enabled = false;
        }
        ConnectButton.interactable = enabled;
    }

    public void UpdateServerText(string address)
    {
        serverAddress.text = address;
    }

    List<GameObject> Serverlines = new List<GameObject>();
    IEnumerator UpdateServerList()
    {
        while (true)
        {

            List<ServerList.Entry> list = LobbyClient.knownServers.list;

            // Server list example script automatically collects servers that have recently announced themselves
            for (int i = 0; i < list.size; ++i)
            {
                GameObject go = null;
                if (Serverlines.Count <= i)
                {
                    go = GameObject.Instantiate(ServerLine) as GameObject;
                    go.transform.SetParent(ServerListRoot.transform);
                    Serverlines.Add(go);
                }
                ServerList.Entry ent = list.buffer[i];
                go = Serverlines.buffer[i];
                go.SetActive(true);
                var button = go.GetComponentInChildren<UnityEngine.UI.Button>();
                var textFields = go.GetComponentsInChildren<Text>();
                textFields[0].text = ent.name + " " + ent.internalAddress;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate
                {
                    Connect(ent.externalAddress, ent.internalAddress);
                });
                /*
                // NOTE: I am using 'internalAddress' here because I know all servers are hosted on LAN.
                // If you are hosting outside of your LAN, you should probably use 'externalAddress' instead.
                if (GUILayout.Button(ent.internalAddress.ToString(), button))
                {
                    TNManager.Connect(ent.internalAddress, ent.internalAddress);
                    mMessage = "Connecting...";
                }
                 */
            }
            // hide any extra lines
            for (int i = list.size; i < Serverlines.Count; i++)
            {
                Serverlines.buffer[i].SetActive(false);
            }
            yield return new WaitForSeconds(.5f);
        }
    }

    void UpdateMenuToggle()
    {
        bool showConnect = true;
        if (TNManager.isConnected)
            showConnect = false;

        joinGroup.alpha = (!showConnect ? 1f : 0f);
        joinGroup.interactable = !showConnect;
        joinGroup.blocksRaycasts = !showConnect;

        connectGroup.alpha = (showConnect ? 1f : 0f);
        connectGroup.interactable = showConnect;
        connectGroup.blocksRaycasts = showConnect;

    }
}
