using System.Collections;
using System.Collections.Generic;
using TNet;
using UnityEngine;

public class WeaponPickupSpawner : PrefabSpawner
{
    [Header("Weapon Pickup Properties")]
    public PickupType Contents;
    public float Value;

    protected override void Awake()
    {
        base.Awake();
        mSpawnFunction = "SpawnWeaponPickupPrefab";
    }

    protected override void Spawn(int channelID)
    {
        if (transform.childCount > 0) return;
        TNManager.Instantiate(channelID, mSpawnFunction, prefab.PathInResources, Persistent, Index, transform.position, transform.rotation, Contents, Value);
    }

    [RCC]
    public static GameObject SpawnWeaponPickupPrefab(GameObject prefab, int spawnerIndex, Vector3 pos, Quaternion rot, PickupType t, float v)
    {
        var go = PrefabSpawner.SpawnPrefab(prefab, spawnerIndex, pos, rot);
        var pickup = go.GetComponent<Pickup>();
        pickup.myType = t;
        pickup.Value = v;
        return go;
    }
}
