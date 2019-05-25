using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TNet;
using UnityEngine.SceneManagement;

public class JoinMenu : MonoBehaviour
{
    public Text PlayerName, ServerName, Messages;
    private string mMessages;
    public GameObject ChannelListRoot;
    public GameObject ChannelLine;

    public LevelInfo level1;
    // Use this for initialization

    private void OnEnable()
    {
        TNManager.onConnect += OnNetworkConnect;
    }

    private void OnDisable()
    {
        TNManager.onConnect -= OnNetworkConnect;
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        PlayerName.text = TNManager.playerName;
        ServerName.text = TNServerInstance.serverName;
    }
    // Update is called once per frame
    void Update()
    {
        if (TNManager.isJoiningChannel)
        {
            mMessages = "  Joining Channel..";
        }

        Messages.text = mMessages;
    }

    public void OnNetworkConnect(bool success, string message)
    {
        if (success)
        {
            Init();
            StartCoroutine("UpdateChannelList");
        }
    }

    public void OnDisconnect()
    {
        StopCoroutine("UpdateChannelList");
        TNManager.Disconnect();
    }

    public void OpenCreateDialog()
    {
        var inputs = level1.GetInputItemList();

        //InputDialogManager.instance.OpenDialog("StartServer", inputs);
        InputDialog.instance.OpenDialog(inputs, delegate { OnCreateRoom(); });

    }

    public void OnCreateRoom()
    {
        var dlg = InputDialog.instance;
        dlg.AcceptValues();
        var servername = dlg.GetValueString(LevelInfo.INPUT_RoomName);
        var maxPlayers = dlg.GetValueInt(LevelInfo.INPUT_MaxPlayers);
        if (!string.IsNullOrEmpty(servername))
            TNServerInstance.serverName = servername;
        if (maxPlayers <= 2)
        {
            maxPlayers = 2;
        }
        var targetSceneName = dlg.GetValueString(LevelInfo.INPUT_LevelName);
        int count = GetComponent<TNChannelList>().Count;
        SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
        TNManager.JoinChannel(count, false, true);
    }

    void OnLevelWasLoaded(int level)
    {
        TNManager.SetTimeout(10); // set it back to 10 after level loads
    }

    List<GameObject> ChannelLines = new List<GameObject>();
    IEnumerator UpdateChannelList()
    {
        var channelList = GetComponent<TNChannelList>();
        while (true)
        {
            var list = channelList.Channels;
            // the channel list script keeps an updated list of the channels
            for (int i = 0; i < list.size; ++i)
            {
                GameObject go = null;
                if (ChannelLines.Count <= i)
                {
                    go = GameObject.Instantiate(ChannelLine) as GameObject;
                    go.transform.SetParent(ChannelListRoot.transform);
                    ChannelLines.Add(go);
                }
                var ent = list[i];
                go = ChannelLines[i];
                go.SetActive(true);
                var button = go.GetComponentInChildren<UnityEngine.UI.Button>();
                var textFields = go.GetComponentsInChildren<Text>();
                var channelData = ent.data; // jsonize at somepoint.. for now just the channel name
                var channelName = (string.IsNullOrEmpty(channelData) ? "loading.." : channelData);
                textFields[0].text = channelName;
                textFields[1].text = ent.level;
                textFields[2].text = "no info right now";
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate
                {
                    Debug.LogFormat("joining id:{0} level:{1}", ent.id, ent.level);
                    TNManager.SetTimeout(120); // stupid levels take forever to load
                    SceneManager.LoadScene("Game", LoadSceneMode.Single);
                    TNManager.JoinChannel(ent.id, false, true);
                    //                    TNManager.JoinChannel(ent.id, ent.level);
                });
            }
            // hide any extra lines
            for (int i = list.size; i < ChannelLines.Count; i++)
            {
                ChannelLines[i].SetActive(false);
            }

            yield return new WaitForSeconds(GetComponent<TNChannelList>().RefreshInterval);
        }
    }
}
