using System.Collections;
using scg = System.Collections.Generic;
using UnityEngine;
using TNet;
using UnityEditor;
[ExecuteInEditMode]
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
        //Debug.LogFormat("adding spawner {0} index  {1}", gameObject.name, Index);
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (preview == null)
            {
                preview = GameObject.Instantiate(prefab.PrefabObject);
                var t = transform;
                preview.hideFlags = HideFlags.HideAndDontSave;
                preview.transform.SetParent(t);
                preview.transform.position = t.position;
                preview.transform.rotation = t.rotation;
                preview.transform.localScale = t.localScale;
            }
        }
#endif
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
        Transform t = go.transform;
        t.position = pos;
        t.rotation = rot;

        if (spawnerIndex < spawners.Count)
        {
            var spawner = PrefabSpawner.spawners[spawnerIndex];
            t.SetParent(spawner.ParentTransform);
        }
        else
        {
            Debug.LogFormat("Spawner index {0} for prefab {1} is invalid", spawnerIndex, prefab.name);
        }
        return go;
    }
}