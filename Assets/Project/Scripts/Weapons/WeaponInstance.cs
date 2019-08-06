using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using Battle;

public class WeaponInstance
{
    [SerializeField]
    public Weapon baseWeapon { get; private set; }
    private Cooldown_counter FireCooldown, ReloadCooldown;
    private Transform raycastOrigin;
    private Camera mainCam;
    private TNObject tno;
    public WeaponPrefab prefabInstance { get; private set; }

    public void SetWeaponPrefab(WeaponPrefab w)
    {
        prefabInstance = w;
        raycastOrigin = w.ShootOrigin;
    }

    public int TotalAmmo { get; set; }
    public int ClipAmmo { get; set; }

    /// <summary>
    /// WeaponTYPEs are about the class of the weapon, not the specific weapon (sniper and rifle are both 'rifle' types as far as the animations are concerned)
    /// </summary>
    /// <returns></returns>
    public int WeaponClassIndex()
    {
        switch (baseWeapon.myType)
        {
            case WeaponType.Pistol:
                return 1;
            case WeaponType.Shotgun:
            case WeaponType.Rifle:
            case WeaponType.Sniper:
                return 3;
            case WeaponType.RocketLauncher:
                return 4;
        }
        return 0;
    }

    public WeaponInstance(Weapon w, Cooldown_counter fire, Cooldown_counter reload, Transform t, TNObject tnObject)
    {
        FireCooldown = fire;
        ReloadCooldown = reload;
        baseWeapon = w;
        raycastOrigin = t;
        mainCam = Camera.main;
        baseCameraFOV = mainCam.fieldOfView;
        tno = tnObject;
    }

    public void AddAmmo(float amount)
    {
        TotalAmmo += (int)amount;
        if (TotalAmmo > baseWeapon.MaxAmmo) TotalAmmo = baseWeapon.MaxAmmo;
        FillClip();
    }

    public void FillClip()
    {
        var clipSpace = baseWeapon.ClipSize - ClipAmmo;
        if (TotalAmmo >= clipSpace)
        {
            ClipAmmo += clipSpace;
            TotalAmmo -= clipSpace;
        }
        else
        {
            ClipAmmo += TotalAmmo;
            TotalAmmo = 0;
        }
    }

    public string GetStatus()
    {
        string fmt = "{0}\r\n clip: {1} reserve: {2}\r\n{3}";
        var cooldownMsg = "";
        if (!CanFireWeapon())
        {
            cooldownMsg = "Cannot fire";
        }
        else if (ReloadCooldown.IsRunning)
        {
            cooldownMsg = "reloading";
            int progress = Convert.ToInt32(10 * ReloadCooldown.Progress);
            if (progress > 0)
            {
                cooldownMsg += "\r\n";
                for(int lcv = 0; lcv < progress; lcv++)
                {
                    cooldownMsg += "-";
                }
            }
        }
        else if (!FireCooldown.IsRunning)
        {
            cooldownMsg = "Can fire";
        }
        else 
        {
            cooldownMsg = "cooling down";
        }
        return string.Format(fmt, baseWeapon.myType.ToString(), ClipAmmo, TotalAmmo, cooldownMsg);
    }

    private bool triggerPressed, triggerReleased;

    private bool canShootSemiAuto;
    private float baseCameraFOV;

    /// <summary>
    /// do any cleanup needed before switching away from this weapon
    /// </summary>
    public void SwitchingOut()
    {
        mainCam.fieldOfView = baseCameraFOV;

    }

    public void Update(float deltatime, RaycastHit lookTargetHit)
    {
        mainCam.fieldOfView = baseCameraFOV;
        // run every time regardless
        if (baseWeapon.ZoomFactor != 0 && Input.GetMouseButton(1))
        {
            mainCam.fieldOfView -= baseWeapon.ZoomFactor;
        }

        // firing related logic.  Reloadrunning cuts that all out
        if (ReloadCooldown.IsRunning)
        {
            return;
        }

        if (BattlePlayerInput.instance.GetKey(KeyCode.R))
        {
            Reload();
            return;
        }

        if (!CanFireWeapon()) { return; }

        var pressingTrigger = BattlePlayerInput.instance.GetAxis("Fire1") > .01f;
        if (!pressingTrigger)
        {
            canShootSemiAuto = true;
        }

        if (!FireCooldown.IsRunning)
        {
            if (BattlePlayerInput.instance.GetAxis("Fire1") > .01f)
            {
                if (ClipAmmo == 0) { Reload(); return; }
                if (!baseWeapon.IsAutomatic)
                {
                    if (!canShootSemiAuto)
                    {
                        Debug.Log("non auto weapon requires trigger release");
                        return;
                    }
                }
                FireCooldown.Start(baseWeapon.CooldownTime);
                BattlePlayer.instance.UpdateActionStance(true);
                Fire(lookTargetHit);
                canShootSemiAuto = false;
            }
        }
        //Debug.DrawLine(raycastOrigin.position, raycastOrigin.forward * 5f, Color.blue);
    }

    public bool CanFireWeapon()
    {
        bool canFire = true;
        if (Mathf.Max(ClipAmmo, TotalAmmo) == 0)
        {
            canFire = false;
        }
        return canFire;
    }

    public void ReloadIfNeeded()
    {
        if (ClipAmmo == 0)
        {
            Reload();
        }
    }


    public Vector3 GetJitter()
    {
        float x, y, z;
        x = UnityEngine.Random.Range(-baseWeapon.AimVariation, baseWeapon.AimVariation);
        y = UnityEngine.Random.Range(-baseWeapon.AimVariation, baseWeapon.AimVariation);
        z = UnityEngine.Random.Range(-baseWeapon.AimVariation, baseWeapon.AimVariation);
        var jitter = new Vector3(x, y, z);
        return jitter;
    }

    public void FireProjectile(Vector3 targetPoint)
    {
        TNManager.Instantiate(tno.channelID, "FireProjectile", baseWeapon.ProjectilePrefab.PathInResources, false, raycastOrigin.position, (targetPoint - raycastOrigin.position).normalized);
    }
    
    public void Fire(RaycastHit lookTargetHit)
    {
        if (baseWeapon.ProjectilePrefab != null)
        {
            FireProjectile(lookTargetHit.point);
            return;
        }
        // else fire with raycasts
        if (lookTargetHit.collider != null)
        {
            Dictionary<int, IDamagable> hitObjects = new Dictionary<int, IDamagable>();
            for (int lcv = 0; lcv < baseWeapon.RaysPerShot; lcv++)
            {
                var targetPos = lookTargetHit.point;
                var dir = (targetPos - raycastOrigin.position).normalized;
                dir += GetJitter();
                dir.Normalize();

                RaycastHit hit;
                if (Physics.Raycast(raycastOrigin.position, dir, out hit))
                {
                    Debug.Log("hit " + hit.collider.gameObject.name);
                    MarkerManager.PlaceMarker(hit.point);
                    var gId = hit.collider.gameObject.GetInstanceID();
                    var destruct = hitObjects.ContainsKey(gId) ? hitObjects[gId] : null;
                    if (hitObjects.ContainsKey(gId) && destruct == null)
                    {
                        // non destructable
                        continue;
                    }
                    else
                    {
                        // need to see if there is a new destructable object hit
                        if (destruct == null) { destruct = hit.collider.gameObject.GetComponent<IDamagable>(); }
                        if (destruct == null) { destruct = hit.collider.GetComponentInParent<IDamagable>(); }
                    }

                    if (baseWeapon.RaysPerShot > 1)
                    {
                        // store object's destructable, if there is one, null if there isn't
                        hitObjects[gId] = destruct;
                    }

                    // apply damage to destructable object
                    if (destruct != null)
                    {
                        float damage = 2;
                        destruct.TakeDamage(damage);
                    }
                }
            }
        }
        Debug.DrawLine(raycastOrigin.position, lookTargetHit.point, Color.magenta,2f);
        UseAmmo(1);
    }

    public void Reload()
    {
        if (ClipAmmo < baseWeapon.ClipSize)
        {
            BattlePlayerInput.instance.tno.Send("SetTimedTrigger", Target.All, "Reload", "ReloadTime", baseWeapon.ReloadTime);
            ReloadCooldown.Start(baseWeapon.ReloadTime, () => { FillClip(); });
        }
    }

    public void UseAmmo(float amount)
    {
        ClipAmmo -= Mathf.Min((int)amount, ClipAmmo);
    }
}
