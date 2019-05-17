using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Pistol, Shotgun, Rifle, Sniper, RocketLauncher, _none};
[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public WeaponType myType;
    public string WeaponName;
    [Header("Capacities")]
    public int ClipSize;
    public int MaxAmmo;
    [Header("Speeds")]
    public float ReloadTime;
    public float CooldownTime;
    [Header("Mechanics")]
    public float MaxRange = 1000;
    public bool IsAutomatic;
    public float AimVariation;
    [Header("Ammo")]
    public int RaysPerShot = 1;
    public Projectile ProjectilePrefab;

    [Header("Scope")]
    public float ZoomFactor = 15;

}
