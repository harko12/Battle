using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using UnityEditor;

public class PrefabSpawner : MonoBehaviour //TNBehaviour
{
    public PrefabPath prefab;
    [HideInInspector]
    public GameObject preview;

    protected string mSpawnFunction = "SpawnPrefab";

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
        TNManager.Instantiate(channelID, mSpawnFunction, prefab.PathInResources, false, transform.position, transform.rotation);
    }

    [RCC]
    public static GameObject SpawnPrefab(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        // Set the position and rotation based on the passed values
        Transform t = go.transform;
        t.position = pos;
        t.rotation = rot;
        return go;
    }
}