﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using Battle;
using Cinemachine;

public class WeaponInstance
{
    [SerializeField]
    public Weapon baseWeapon { get; private set; }
    private Cooldown_counter FireCooldown, ReloadCooldown;
    public bool isReady;
    public bool isAiming;
    private Transform raycastOrigin;
    public CinemachineVirtualCamera aimCamera;
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
        tno = tnObject;
    }

    public void SetAimCamera(CinemachineVirtualCamera aimCam)
    {
        aimCamera = aimCam;
        baseCameraFOV = aimCamera.m_Lens.FieldOfView;
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
        AdjustAimFieldOfView(0);

    }

    private void AdjustAimFieldOfView(float fov)
    {

        aimCamera.m_Lens.FieldOfView = baseCameraFOV + fov;
    }

    private int shotsFired = 0;

    public void Update(float deltatime, RaycastHit lookTargetHit)
    {
        //AdjustAimFieldOfView(0);
        // run every time regardless
        if (baseWeapon.ZoomFactor != 0 && isAiming)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                AdjustAimFieldOfView(-baseWeapon.ZoomFactor);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                AdjustAimFieldOfView(0);
            }
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
            shotsFired = 0;
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
                        //Debug.Log("non auto weapon requires trigger release");
                        return;
                    }
                }
                FireCooldown.Start(baseWeapon.CooldownTime);
                BattlePlayer.instance.UpdateActionStance(true);
                shotsFired++;
                Fire(lookTargetHit);
                canShootSemiAuto = false;
            }
        }
        //Debug.DrawLine(raycastOrigin.position, raycastOrigin.forward * 5f, Color.blue);
    }

    public bool CanFireWeapon()
    {
        if (!isReady) return false;

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


    public Vector3 GetJitter(int shots)
    {
        var variation = baseWeapon.AimVariation; // + (.05f * shots);
        float x, y, z;
        x = UnityEngine.Random.Range(-variation, variation);
        y = UnityEngine.Random.Range(-variation, variation);
        z = UnityEngine.Random.Range(-variation, variation);
        var jitter = new Vector3(x, y, z);
        return jitter;
    }

    public void FireProjectile(Vector3 targetPoint)
    {
        TNManager.Instantiate(tno.channelID, "FireProjectile", baseWeapon.ProjectilePrefab.PathInResources, false, raycastOrigin.position, (targetPoint - raycastOrigin.position).normalized);
    }
    
    public void Fire(RaycastHit lookTargetHit)
    {
        prefabInstance.tno.SendQuickly("WeaponFireEffects", Target.All);
        tno.SendQuickly("Recoil", Target.All, 1);
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
                dir += GetJitter(shotsFired);
                dir.Normalize();

                RaycastHit hit;
                if (Physics.Raycast(raycastOrigin.position, dir, out hit))
                {
                    //Debug.Log("hit " + hit.collider.gameObject.name);
                    var impactRot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
                    var impactType = ImpactTypes.Dirt;
//                    MarkerManager.PlaceMarker(hit.point);
                    var gId = hit.collider.gameObject.GetInstanceID();
                    var destruct = hitObjects.ContainsKey(gId) ? hitObjects[gId] : null;
                    if (hitObjects.ContainsKey(gId) && destruct == null)
                    {
                        // non destructable
                        BattleGameFxManager.instance.SpawnImpact(impactType, hit.point, impactRot);
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
                        impactType = destruct.GetImpactType();
                    }
                    BattleGameFxManager.instance.SpawnImpact(impactType, hit.point, impactRot);
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
