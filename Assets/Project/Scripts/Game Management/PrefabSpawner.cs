using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using UnityEditor;

public class PrefabSpawner : TNBehaviour
{
    // prefab preview code taken from http://https://github.com/editkid/unity3d-gizmo-mesh-preview

    // The prefab that we want to draw gizmo meshes for
    public GameObject prefab;
    public string prefabPath;
    public GameObject preview;

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
        if (!TNManager.IsHosting(tno.channelID))
        {
            return;
        }
        var parent = prefab.transform.root.gameObject;
        /*
        var path = AssetDatabase.GetAssetPath(parent);
        path = path.Replace("Assets/Project/Resources/", "");
        path = path.Replace(".prefab", "");
        */
        var path = prefabPath;
        TNManager.Instantiate(channelID, "SpawnPrefab", path, false,  transform.position, transform.rotation);
    }

    [RCC]
    static GameObject SpawnPrefab(GameObject prefab, Vector3 pos, Quaternion rot)
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