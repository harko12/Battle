﻿using System.Collections;
using SCG = System.Collections.Generic;
using System.Linq;
using TNet;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class BattlePlayer : TNBehaviour, IPickupCollector, IDamagable
{
    public static BattlePlayer instance;
    public GameEvents gameEvents;
    [SerializeField] public UIValues uiValues;
    public enum PlayerTool
    {
        Pickaxe,
        Obstacle,
        None
    }

    [Header("Focal Point variables")]
    public GameObject rotationPoint;
    public GameObject focalPoint;
    [SerializeField] private float focalDistance;
    [SerializeField] private float focalSmoothness;
    [SerializeField] private KeyCode changeFocalSideKey;

    [Header("Interaction")]
    [SerializeField]
    private GameCamera gameCamera;
    public KeyCode InteractionKey;
    public KeyCode ToolSwitchKey;
    public float InteractionDistance;

    [Header("Gameplay")]
    [SerializeField]
    public PlayerTool Tool;
    [SerializeField]
    private int _resources;
    public int Resources
    {
        get
        {
            return _resources;
        }
        set
        {
            _resources = value;
            uiValues.ResourceCount = _resources;
        }
    }

    [SerializeField]
    private float _health;
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }
    private bool waitingToRespawn = false;

    public Cooldown PickCooldown;
    [SerializeField]
    private GameObject ObstacleParent;
    private Obstacle[] ObstacleScripts;
    public PrefabPath[] ObstaclePrefabs;
    private int lastObstacleIndex;
    public GameObject ObstaclePreview;
    private Mesh ObstaclePreviewMesh;
    public Cooldown ObstacleCooldown;
    [Header("Weapons")]
    public Transform PistolHolsterMountPoint;
    public Transform HeldWeaponMountPoint;
    [SerializeField]
    private WeaponInstance currentWeapon;
    private SCG.List<WeaponInstance> weapons;
    private Cooldown_counter WeaponFireCooldown, WeaponReloadCooldown, WeaponEquipCooldown;
    public Transform shootOrigin;

    private bool isFocalPointOnLeft = true;

    protected override void Awake()
    {
        base.Awake();
        if (tno.isMine)
        {
            BattlePlayer.instance = this;
        }
    }

    private void OnEnable()
    {
        //gameEvents.OnToolChanged.AddListener(onToolChanged);
    }

    private void OnDisable()
    {
        //gameEvents.OnToolChanged.RemoveListener(onToolChanged);
    }

    // Use this for initialization
    void Start()
    {
        waitingToRespawn = true;
        Init();
    }

    public void Init()
    {
        gameCamera = Camera.main.GetComponent<GameCamera>();
        ObstacleParent = GameObject.FindGameObjectWithTag("ObstacleParent");
        weapons = new SCG.List<WeaponInstance>();
        WeaponFireCooldown = new Cooldown_counter();
        WeaponReloadCooldown = new Cooldown_counter();
        WeaponEquipCooldown = new Cooldown_counter();
        PickCooldown.Reset();
        ObstacleCooldown.Reset();
        SetTool(PlayerTool.None);
        ObstaclePreviewMesh = ObstaclePreview.GetComponentInChildren<MeshFilter>().mesh;
        Respawn();
    }

    public void Respawn()
    {
        if (tno.isMine)
        {
            //Cursor.lockState = CursorLockMode.Locked;
            gameCamera.Init(this);
        }
        Health = 50;
        Resources = 0;// to init the counter
        waitingToRespawn = false;

    }

    [RFC]
    public void Die()
    {
        if (!tno.isMine) { return; }
        waitingToRespawn = true;
        gameCamera.Release();
        gameEvents.OnPlayerDeath.Invoke(this);
    }

    private void OnDestroy()
    {
        Destroy(ObstaclePreviewMesh);
    }
    void updateCooldowns()
    {
        WeaponFireCooldown.Tick(Time.deltaTime);
        WeaponReloadCooldown.Tick(Time.deltaTime);
        WeaponEquipCooldown.Tick(Time.deltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        if (!tno.isMine || gameCamera == null || waitingToRespawn) { return; }
        //handleCursorMode();

        updateCooldowns();
        uiValues.ToolMessage = "";
        uiValues.WeaponStatus = "";
        uiValues.PlayerHealth = Health;
        if (Input.GetKeyDown(changeFocalSideKey))
        {
            isFocalPointOnLeft = !isFocalPointOnLeft;
        }

        float targetX = focalDistance * (isFocalPointOnLeft ? -1 : 1);
        float smoothX = Mathf.Lerp(focalPoint.transform.localPosition.x, targetX, focalSmoothness * Time.deltaTime);
        focalPoint.transform.localPosition = new Vector3(smoothX, focalPoint.transform.localPosition.y, focalPoint.transform.localPosition.z);

        // interaction
        var cameraTransform = gameCamera.transform;
        var cameraAnchor = gameCamera.GetRotationAnchor();
        Ray r = new Ray(cameraAnchor.position, cameraTransform.forward);
#if UNITY_EDITOR
        Debug.DrawLine(cameraAnchor.position, cameraTransform.position + cameraTransform.forward * InteractionDistance, Color.green);
#endif
        // get any interactable in our range (only one)

        RaycastHit hit;
        IInteractable interactable = null;
        if (Physics.Raycast(r, out hit, 5f))
        {
            interactable = hit.collider.GetComponentInParent<IInteractable>();
        }

        switch (Tool)
        {
            case PlayerTool.None:
                break;
            case PlayerTool.Pickaxe:
                UpdatePickAxe(interactable);
                break;
            case PlayerTool.Obstacle:
                UpdateObstacleTool();
                break;
        }

        handleInteractionKey(interactable);

        if (Input.GetKeyDown(ToolSwitchKey))
        {
            CycleTool();
        }

        UpdateWeapon();

    }
    /*
    void handleCursorMode()
    {
        if (Input.GetKey(KeyCode.LeftBracket))
        {
            if (Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    */
    private bool weaponSwitching = false;
    void handleWeaponSwitchKeys()
    {
        if (weaponSwitching) return;
        for (int lcv = 1, length = weapons.Count; lcv <= length; lcv++)
        {
            if (lcv > 9) { break; } // max 9 for now
            if (Input.GetKeyDown(lcv.ToString()))
            {
                SwitchWeapon(lcv);
            }
        }
    }

    void UpdateWeapon()
    {
        handleWeaponSwitchKeys();
        if (currentWeapon != null)
        {
            var distanceFromCamera = Vector3.Distance(gameCamera.transform.position, shootOrigin.transform.position);
            RaycastHit hit;
            Physics.Raycast((gameCamera.transform.position + gameCamera.transform.forward * distanceFromCamera), gameCamera.transform.forward, out hit);
            currentWeapon.Update(Time.deltaTime, hit);
            uiValues.WeaponStatus = currentWeapon.GetStatus();
        }
    }

    void SwitchWeapon(int weaponType)
    {
        int index = weaponType - 1;
        if (currentWeapon != null)
        {
            currentWeapon.SwitchingOut();
            if (currentWeapon.WeaponTypeIndex() == weaponType) { index = -1; } // reclickng the same weapon key unequps the weapon for now
        }

        if (index < 0) // setting weapon to none
        {
            currentWeapon = null;
        }
        else
        {
            SetTool(PlayerTool.None);
            currentWeapon = weapons[index];
        }
        weaponSwitching = true;
        WeaponEquipCooldown.Start(2, () => { weaponSwitching = false; });
        var wpTypeIndex = currentWeapon != null ? currentWeapon.WeaponTypeIndex() : 0;
        Battle.BattlePlayerInput.instance.tno.Send("SetWeapon", Target.AllSaved, wpTypeIndex);
    }

    void handleInteractionKey(IInteractable i)
    {
        if (i != null && i.CanInteract(this))
        {
            if (Input.GetKeyDown(InteractionKey))
            {
                i.Interact(this);
            }
        }
    }

    void UpdatePickAxe(IInteractable i)
    {
        if (i != null && i.CanInteract(this))
        {
            if (i is ResourceObject)
            {
                if (!PickCooldown.IsRunning)
                {
                    if (Input.GetAxis("Fire1") > 0)
                    {
                        StartCoroutine(PickCooldown.Run());
                        i.Interact(this);
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(InteractionKey))
                {
                    i.Interact(this);
                }
            }
        }
    }
    void UpdateObstacleScripts()
    {
        var scripts = new List<Obstacle>();
        foreach (var prefab in ObstaclePrefabs)
        {
            scripts.Add(prefab.PrefabObject.GetComponent<Obstacle>());
        }
        ObstacleScripts = scripts.ToArray();
    }
    void UpdateObstacleTool()
    {
        if (ObstaclePrefabs.Length < 1)
        {
            return;
        }

        if (ObstacleScripts == null ||  ObstaclePrefabs.Length != ObstacleScripts.Length)
        {
            UpdateObstacleScripts();
        }

        var index = lastObstacleIndex;
        var mouseScroll = Input.mouseScrollDelta.y;

        if (mouseScroll > 0)
        {
            index = CycleArrayIndex(index, ObstaclePrefabs.Length, 1);
        }
        else if (mouseScroll < 0)
        {
            index = CycleArrayIndex(index, ObstaclePrefabs.Length, -1);
        }

        var currentObsPrefab = ObstaclePrefabs[index];
        var currentObsScript = ObstacleScripts[index];
        uiValues.ToolMessage = string.Format("{0} costs {1}/{2}", currentObsScript.name, currentObsScript.Cost, Resources);
        var currentMesh = currentObsPrefab.PrefabObject.GetComponentInChildren<MeshFilter>();
        var verts = currentMesh.sharedMesh.vertices;
        ObstaclePreview.transform.localScale = currentMesh.transform.localScale;
        ObstaclePreviewMesh.vertices = verts;
        ObstaclePreviewMesh.RecalculateNormals();

        RaycastHit hit;
        var ray = new Ray(focalPoint.transform.position + (Vector3.up * 200), Vector3.down);
        if (Physics.Raycast(ray, out hit))
        {
            ObstaclePreview.transform.position = hit.point;
        }

        lastObstacleIndex = index;
        if (Input.GetAxis("Fire1") > 0)
        {
            PlaceObstacle(currentObsPrefab, currentObsScript, ObstaclePreview.transform);
            StartCoroutine(ObstacleCooldown.Run());
        }
    }

    void PlaceObstacle(PrefabPath p, Obstacle obs, Transform obsPosition)
    {
        if (ObstacleCooldown.IsRunning) { return; }
        if (Resources < obs.Cost)
        {
            return;
        }

        Resources -= obs.Cost;
//        var newObs = Instantiate(p, obsPosition.position, obsPosition.rotation, ObstacleParent.transform);
        TNManager.Instantiate(tno.channelID, "CreateObstacle", p.PathInResources, false, obsPosition.position, obsPosition.rotation);

    }

    public void AddResource(int r)
    {
        Resources += r;
    }

    void SetTool(PlayerTool newTool)
    {
        if (newTool != PlayerTool.None)
        {
            SwitchWeapon(-1);
        }
        Tool = newTool;
        ObstaclePreview.gameObject.SetActive(Tool == PlayerTool.Obstacle);
        gameEvents.OnToolChanged.Invoke(Tool);
    }

    private int CycleArrayIndex(int current, int length, int dir)
    {
        var newIndex = current + dir;
        if (newIndex > length - 1)
        {
            newIndex -= length;
        }
        else if (newIndex < 0)
        {
            newIndex = length + newIndex;
        }
        return newIndex;
    }

    void CycleTool()
    {
        int currentTool = (int)Tool;
        int nextTool = -1;
        var values = (PlayerTool[])System.Enum.GetValues(typeof(PlayerTool));
        int firstTool = (int)values[0];
        for (int lcv = 0, length = values.Length; lcv < length; lcv++)
        {
            var val = values[lcv];
            if ((int)val == currentTool)
            {
                if (lcv < length - 1)
                {
                    nextTool = (int)values[lcv + 1];
                    break;
                }
                else
                {
                    nextTool = firstTool;
                    break;
                }
            }
        }
        SetTool((PlayerTool)nextTool);
    }

    public bool PickedUp(Pickup p)
    {
        bool pickedUp = false;
        switch (p.myType)
        {
            case PickupType.Ammo:
                pickedUp = AddAmmoFromPickup(p);
                break;
            case PickupType.Weapon:
                pickedUp = AddWeaponFromPickup(p);
                break;
        }
        return pickedUp;
    }

    public void DrawWeapon(int weaponType)
    {
        if (!tno.isMine) return;
        Debug.Log("drawing Weapon");
        tno.Send("PlacePrefab", Target.AllSaved, currentWeapon.prefabInstance.tno.uid, WeaponMountPoints.RightHand);
/*
        var t = currentWeapon.prefabInstance.transform;
        t.position = HeldWeaponMountPoint.position;
        t.rotation = HeldWeaponMountPoint.rotation;
        t.SetParent(HeldWeaponMountPoint);
        */
    }

    public void PlaceWeapon(int weaponType)
    {
        if (!tno.isMine) return;
        Debug.Log("Holstering Weapon");
        var prevWeapon = weapons.Where(w => w.WeaponTypeIndex() == weaponType).FirstOrDefault();
        if (prevWeapon != null && prevWeapon.prefabInstance != null)
        {
            tno.Send("PlacePrefab", Target.AllSaved, prevWeapon.prefabInstance.tno.uid, WeaponMountPoints.Pistol);
/*
            var t = prevWeapon.prefabInstance.transform;
            t.position = PistolHolsterMountPoint.position;
            t.rotation = PistolHolsterMountPoint.rotation;
            t.SetParent(PistolHolsterMountPoint);
            */
        }
    }

    public enum WeaponMountPoints { RightHand, Pistol, Auto, Sniper, RPG};
    [RFC]
    public void PlacePrefab(uint prefabID, int mountPoint)
    {
        Transform parent = transform;
        switch((WeaponMountPoints)mountPoint)
        {
            case WeaponMountPoints.Pistol:
                parent = PistolHolsterMountPoint;
                break;
            case WeaponMountPoints.RightHand:
                parent = HeldWeaponMountPoint;
                break;
            default:
                break;
        }
        var prefab = TNObject.Find(tno.channelID, prefabID);
        prefab.transform.SetParent(parent);
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
        //        prefab.transform.position = parent.position;
        //        prefab.transform.rotation = parent.rotation;
    }

    private bool AddWeaponFromPickup(Pickup p)
    {
        var weaponToAdd = weapons.Where(w => w.baseWeapon.myType == p.baseWeapon.myType).FirstOrDefault();
        if (weaponToAdd == null)
        {
            weaponToAdd = new WeaponInstance(p.baseWeapon, WeaponFireCooldown, WeaponReloadCooldown, shootOrigin, tno);
            weapons.Add(weaponToAdd);
            weaponToAdd.prefabInstance = p.GetComponent<WeaponPrefab>();
            if (weaponToAdd.prefabInstance != null)
            {
                tno.Send("PlacePrefab", Target.AllSaved, weaponToAdd.prefabInstance.tno.uid, WeaponMountPoints.Pistol);
            }
        }
        weaponToAdd.AddAmmo(p.Value);
        uiValues.WeaponStatus = weaponToAdd.GetStatus();
        return true;
    }

    private bool AddAmmoFromPickup(Pickup p)
    {
        var pickedUp = false;
        var currentWeapon = weapons.Where(w => w.baseWeapon.myType == p.baseWeapon.myType).FirstOrDefault();
        if (currentWeapon != null)
        {
            currentWeapon.AddAmmo(p.Value);
            pickedUp = true;
            uiValues.WeaponStatus = currentWeapon.GetStatus();
        }
        return pickedUp;
    }

    public void TakeDamage(float damageAmount)
    {
        tno.Send("ApplyDamage", Target.AllSaved, damageAmount);
    }

    [RFC]
    public void ApplyDamage(float damageAmount)
    {
        Health -= damageAmount;
        uiValues.PlayerHealth = Health;
        CheckDamage();
    }

    private void CheckDamage()
    {
        if (Health <= 0)
        {
            tno.Send("Die", Target.AllSaved);
//            Die();
        }
    }
}
