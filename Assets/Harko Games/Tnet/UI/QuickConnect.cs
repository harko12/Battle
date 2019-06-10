using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using System.Net;

public class QuickConnect : MonoBehaviour
{
    public string ServerAddress;
    public string PlayerName;
    public bool JoinPeristent;
    public int Channel = 1;

    private void OnEnable()
    {
        TNManager.onConnect += OnNetworkConnect;
        TNManager.onJoinChannel += OnJoinChannel;
    }

    private void OnDisable()
    {
        TNManager.onConnect -= OnNetworkConnect;
        TNManager.onJoinChannel -= OnJoinChannel;
    }

    // Start is called before the first frame update
    void Start()
    {
        TryConnecting();
    }

    void TryConnecting()
    {
        IPEndPoint extAddress, intAddress;
        extAddress = intAddress = Tools.ResolveEndPoint(ServerAddress);
        TNManager.playerName = PlayerName;
        TNManager.Connect(extAddress, intAddress);
    }

    void OnNetworkConnect(bool success, string error)
    {
        if (success)
        {
            TNManager.JoinChannel(Channel, JoinPeristent, true);
        }
    }

    void OnJoinChannel(int channelID, bool success, string message)
    {
        if (success)
        {
            Debug.LogFormat("Joined channel {0}", channelID);
        }
        else
        {
            Debug.LogFormat("Failed to join channel: {0}", message);
        }
    }
    public bool TryToConnect = false;
    // Update is called once per frame
    void Update()
    {
        if (TNManager.isTryingToConnect || TNManager.isConnected)
        {
            return;
        }

        if (TryToConnect)
        {
            TryConnecting();
            TryToConnect = false;
        }
    }
}
