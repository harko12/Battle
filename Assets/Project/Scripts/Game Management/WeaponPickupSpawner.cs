using System.Collections;
using System.Collections.Generic;
using TNet;
using UnityEngine;

public class WeaponPickupSpawner : PrefabSpawner
{
    [Header("Weapon Pickup Properties")]
    public PickupType Contents;
    public float Value;

    private void Awake()
    {
        mSpawnFunction = "SpawnWeaponPickupPrefab";
    }

    protected override void Spawn(int channelID)
    {
        TNManager.Instantiate(channelID, mSpawnFunction, prefab.PathInResources, false, transform.position, transform.rotation, Contents, Value);
    }

    [RCC]
    public static GameObject SpawnWeaponPickupPrefab(GameObject prefab, Vector3 pos, Quaternion rot, PickupType t, float v)
    {
        var go = PrefabSpawner.SpawnPrefab(prefab, pos, rot);
        var pickup = go.GetComponent<Pickup>();
        pickup.myType = t;
        pickup.Value = v;
        return go;
    }


}
