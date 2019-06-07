using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

public enum PickupType { Ammo, Health, Shield, Weapon };

public class Pickup : TNBehaviour
{
    public float PickupRadius;
    public PickupType myType;
    public Weapon baseWeapon;
    public float Value;

    [Header("Pickup Movement")]
    private Vector3 offset;
    public Transform pickupTransform;
    //public WeaponPrefab prefab { get; private set; }
 
    private float verticalAngle;
    public float rotationAngle;

    public float verticalRange, verticalSpeed;

    // Start is called before the first frame update
    void Start()
    {
        var coll = gameObject.AddComponent<CapsuleCollider>();
        coll.height = verticalRange;
        coll.center = new Vector3(0, verticalRange * .5f, 0);
        coll.radius = PickupRadius;
        coll.isTrigger = true;
        offset = new Vector3(0, verticalRange + pickupTransform.localScale.y, 0); 
        /*
        // the localscale thing is because it is moving base on the 'center' of the cube.  the scale tells us how much height from the 'bottom' to the 'center'
        if (prefab == null && baseWeapon.WeaponPrefab.PrefabObject != null)
        {
            TNManager.Instantiate(tno.channelID,"CreatePickupPrefab", "Prefabs/Weapons/WeaponPack_glock (battle)", true, tno.uid);
            var wp =   Instantiate<WeaponPrefab>(baseWeapon.WeaponPrefab, pickupTransform);
            wp.transform.position = pickupTransform.position;
            wp.transform.rotation = pickupTransform.rotation;
            prefab = wp;
        }
        */
    }
    /*
    [RCC]
    public static GameObject CreatePickupPrefab(GameObject prefab, uint pickupID)
    {
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        var parentObj = TNObject.Find(pickupID);
        var p = parentObj.GetComponent<Pickup>();
        var parent = p.pickupTransform; // it's mounting the whole spawner, not just the pistol part
        go.transform.SetParent(parent);
        go.transform.position = parent.position;
        go.transform.rotation = parent.rotation;
        return go;
    }
    */

    // Update is called once per frame
    void Update()
    {
        if (offset.y != verticalRange) { offset.Set(0, verticalRange + pickupTransform.localScale.y, 0); }
        verticalAngle += verticalSpeed * Time.deltaTime;
        var newPos = offset + new Vector3(0, Mathf.Cos(verticalAngle) * verticalRange, 0);
//        var newPos = offset + new Vector3(0, Mathf.Clamp(Mathf.Cos(verticalAngle) * verticalRange, 0, verticalRange), 0);
        pickupTransform.localPosition = newPos;
        pickupTransform.Rotate(0, rotationAngle * Time.deltaTime, 0);
    }

    [RFC]
    public void Die()
    {
        this.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var pickupCollector = other.GetComponent<IPickupCollector>();
        if (pickupCollector != null)
        {
            var playerTno = other.gameObject.GetComponent<TNObject>();
            if (playerTno.isMine)
            {
                var success = pickupCollector.PickedUp(this);
                if (success)
                {
                    tno.Send("Die", Target.AllSaved);
                }
            }
        }
    }
}
