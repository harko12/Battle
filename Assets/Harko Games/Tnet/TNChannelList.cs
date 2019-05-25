using UnityEngine;
using System.Collections;
using TNet;
using System.IO;
using System.Net;

public class TNChannelList : MonoBehaviour {

    public float RefreshInterval = 1;
    public int Count { 
        get
        {
            return AllActiveChannels.Count;
        }
    }

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
        refreshTime = Time.time;
	}

    private float refreshTime;

	// Update is called once per frame
	void Update () {
        if (Time.time > refreshTime)
        {
            if (TNManager.isConnected)
            {
                TNManager.client.BeginSend(Packet.RequestChannelList);
                TNManager.client.EndSend();
            }
            refreshTime = Time.time + RefreshInterval;
        }
	}

    private List<ChannelEntry> AllActiveChannels = new List<ChannelEntry>();

    public List<ChannelEntry> Channels
    {
        get
        {
            return AllActiveChannels;
        }
    }

    void OnNetworkConnect(bool success, string message)
    {
        TNManager.client.packetHandlers[(byte)Packet.ResponseChannelList] = OnChannelList;
    }

    void OnChannelList(Packet response, BinaryReader reader, IPEndPoint source)
    {
        AllActiveChannels.Clear();
        int count = reader.ReadInt32();
        for (int i = 0; i < count; ++i)
        {
            ChannelEntry _newChannelEntry = new ChannelEntry();
            _newChannelEntry.id = reader.ReadInt32();
            _newChannelEntry.count = reader.ReadUInt16();
            _newChannelEntry.limit = reader.ReadUInt16();
            _newChannelEntry.password = reader.ReadBoolean();
            _newChannelEntry.persistent = reader.ReadBoolean();
            _newChannelEntry.level = reader.ReadString();
            _newChannelEntry.data = reader.ReadString();
            AllActiveChannels.Add(_newChannelEntry);
        }
    }


    public struct ChannelEntry
    {
        public int id, count, limit;
        public bool password, persistent;
        public string level, data;
    }

}
