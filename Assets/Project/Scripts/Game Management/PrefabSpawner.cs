using System.Collections;
using scg = System.Collections.Generic;
using UnityEngine;
using TNet;
using UnityEditor;

public class PrefabSpawner : MonoBehaviour
{
    public static int lastId = -1;
    public static scg.List<PrefabSpawner> spawners = new scg.List<PrefabSpawner>();

    public PrefabPath prefab;
    public Transform ParentTransform;
    public bool Persistent = false;
    [HideInInspector]
    public GameObject preview;
    [SerializeField]
    protected int Index;

    protected string mSpawnFunction = "SpawnPrefab";

    protected virtual void Awake()
    {
        PrefabSpawner.spawners.Add(this);
        Index = PrefabSpawner.spawners.IndexOf(this);
        Debug.LogFormat("{0} setting index to {1}", gameObject.name, Index);
    }

    private void OnEnable()
    {
        TNManager.onJoinChannel += OnJoinChannel;
    }

    private void OnDisable()
    {
        TNManager.onJoinChannel -= OnJoinChannel;
    }

    protected void OnJoinChannel(int channelID, bool success, string message)
    {
        if (!TNManager.IsHosting(channelID))
        {
            return;
        }
        if (prefab == null)
        {
            Debug.LogErrorFormat("No prefab assigned for {0}", gameObject.name);
            return;
        }
        Spawn(channelID);
    }

    protected virtual void Spawn(int channelID)
    {
        if (transform.childCount > 0) return;
        TNManager.Instantiate(channelID, mSpawnFunction, prefab.PathInResources, Persistent, Index, transform.position, transform.rotation);
    }

    [RCC]
    public static GameObject SpawnPrefab(GameObject prefab, int spawnerIndex, Vector3 pos, Quaternion rot)
    {
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        // Set the position and rotation based on the passed values
        var spawner = PrefabSpawner.spawners[spawnerIndex];
        Transform t = go.transform;
        t.position = pos;
        t.rotation = rot;
        t.SetParent(spawner.ParentTransform);
        return go;
    }
}