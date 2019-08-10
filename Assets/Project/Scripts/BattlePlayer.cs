using SCG = System.Collections.Generic;
using System.Linq;
using TNet;
using UnityEngine;

namespace Battle
{
            public enum WeaponMountPoints { RightHand, Pistol, Rifle, Auto, Sniper, RPG };


    public class BattlePlayer : TNBehaviour, IPickupCollector, IDamagable
    {
        public static BattlePlayer instance;
        private GameEvents gameEvents;
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
        public float focalSmoothness;
        public KeyCode changeFocalSideKey;

        [Header("Interaction")]
        public GameCamera[] GameCameras;
        private GameCamera gameCamera;
        public KeyCode InteractionKey;
        public KeyCode ToolSwitchKey;
        public float InteractionDistance;
        [Header("Camera")]
        [SerializeField]
        public Transform aimTarget;
        [SerializeField]
        private Cinemachine.CinemachineVirtualCamera aimCamera;
        [SerializeField]
        private Cinemachine.CinemachineVirtualCamera followCamera;
        [Header("IK solvers")]
        [SerializeField]
        private RootMotion.FinalIK.AimIK aimIk;
        [SerializeField]
        private RootMotion.FinalIK.LookAtIK lookIk;
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
        public Transform[] MountPoints;
        public Transform HeldWeaponMountPoint;
        public bool InActionStance { get; set; }
        public float ActionStanceTime = 10;
        private WeaponInstance currentWeapon;
        private WeaponInstance previousWeapon;
        private SCG.List<WeaponInstance> weapons;
        private Cooldown_counter WeaponFireCooldown, WeaponReloadCooldown, WeaponEquipCooldown, InActionStanceCooldown;
        public Transform shootOrigin;
        private bool isAiming;
        private bool isFocalPointOnLeft = true;

        protected override void Awake()
        {
            base.Awake();
            gameEvents = BattleGameObjects.instance.gameEvents;
            if (tno.isMine)
            {
                BattlePlayer.instance = this;
                gameCamera = GameCameras[0];
                tno.Send("ToggleAim", Target.AllSaved, false);
            }
        }

        [RFC]
        public void ToggleAim(bool aiming)
        {
            var gameCameraIndex = 1;
            var aimCamPriority = 10;
            var lookCamPriority = 0;
            if (!aiming)
            {
                gameCameraIndex = 0;
                aimCamPriority = 0; lookCamPriority = 10;
            }
            gameCamera = GameCameras[gameCameraIndex];
            isAiming = aiming;

            if (tno.isMine)
            {
                aimCamera.Priority = aimCamPriority;
                followCamera.Priority = lookCamPriority;
            }
        }

        // Use this for initialization
        void Start()
        {
            waitingToRespawn = true;
            Init();
        }

        public void Init()
        {
            ObstacleParent = GameObject.FindGameObjectWithTag("ObstacleParent");
            weapons = new SCG.List<WeaponInstance>();
            WeaponFireCooldown = new Cooldown_counter();
            WeaponReloadCooldown = new Cooldown_counter();
            WeaponEquipCooldown = new Cooldown_counter();
            InActionStanceCooldown = new Cooldown_counter();
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
                BattleGameCameraManager.instance.PlayerSpawned(this);
                //            lookIk.solver.target = BattleGameCameraManager.instance.FreeLookDefaultTarget;
                //            gameCamera.Init(this);
            }
            else
            {
                aimCamera.Priority = 0;
                followCamera.Priority = 0;
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
            InActionStanceCooldown.Tick(Time.deltaTime);
        }

        void LateUpdate()
        {
            if (!tno.isMine) { return; }
            UpdateIK();
            /*
                    if (currentWeapon != null)
                    {
                        aimIk.solver.axis = aimIk.solver.transform.InverseTransformDirection(currentWeapon.prefabInstance.transform.forward);
                    }
                    */
        }

        [RFC]
        public void SetAimTargetPosition(Vector3 pos)
        {
            aimTarget.position = pos;
        }
        public float weaponRange = 100f;

        // Update is called once per frame
        void Update()
        {
            if (!tno.isMine || gameCamera == null || waitingToRespawn) { return; }

            updateCooldowns();
            uiValues.ToolMessage = "";
            uiValues.WeaponStatus = "";
            uiValues.PlayerHealth = Health;
            if (Input.GetKeyDown(changeFocalSideKey))
            {
                isFocalPointOnLeft = !isFocalPointOnLeft;
            }

            float targetX = gameCamera.focalDistance * (isFocalPointOnLeft ? -1 : 1);
            float smoothX = Mathf.Lerp(focalPoint.transform.localPosition.x, targetX, focalSmoothness * Time.deltaTime);
            focalPoint.transform.localPosition = new Vector3(smoothX, focalPoint.transform.localPosition.y, focalPoint.transform.localPosition.z);

            // interaction
            var cameraTransform = gameCamera.transform;
            var cameraAnchor = gameCamera.GetRotationAnchor();
            var aimTargetPos = cameraTransform.position + (cameraTransform.forward * weaponRange);
            SetAimTargetPosition(aimTargetPos);
            tno.SendQuickly("SetAimTargetPosition", Target.Others, aimTargetPos);
            //        Debug.DrawLine(cameraTransform.position, aimTargetPos, Color.blue);
#if UNITY_EDITOR
#endif
            // get any interactable in our range (only one)

            RaycastHit hit;
            IInteractable interactable = null;
            Ray r = new Ray(cameraAnchor.position, cameraTransform.forward);
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

        private bool weaponSwitching = false;
        void handleWeaponSwitchKeys()
        {
            if (weaponSwitching) return;
            foreach (WeaponType wt in System.Enum.GetValues(typeof(WeaponType)))
            {
                var lcv = (int)wt;
                if (Input.GetKeyDown(lcv.ToString()))
                {
                    SwitchWeapon(lcv);
                    break;
                }
            }
        }

        void UpdateIK()
        {
            var lookWeight = 0;
            var aimWeight = currentWeapon != null && (InActionStance && !weaponSwitching) && (int)currentWeapon.baseWeapon.myType > 0 ? 1 : 0;
            tno.SendQuickly("SetIKWeights", Target.All, aimWeight, lookWeight);
        }

        [RFC]
        public void SetIKWeights(float targetAimWeight, float targetLookWeight)
        {
            var currentAimW = aimIk.solver.IKPositionWeight;
            var aimW = Mathf.Lerp(currentAimW, targetAimWeight, Time.deltaTime * focalSmoothness);
            aimIk.solver.IKPositionWeight = aimW;

            lookIk.solver.IKPositionWeight = targetLookWeight;
        }

        void UpdateWeapon()
        {
            handleWeaponSwitchKeys();
            if (currentWeapon != null)
            {
                var distanceFromCamera = Vector3.Distance(gameCamera.transform.position, shootOrigin.transform.position);
                RaycastHit hit;
                Physics.Raycast((gameCamera.transform.position + gameCamera.transform.forward * distanceFromCamera), gameCamera.transform.forward, out hit);
                if (isAiming)
                {
                    UpdateActionStance(true);
                }
                currentWeapon.Update(Time.deltaTime, hit);
                uiValues.WeaponStatus = currentWeapon.GetStatus();
            }
        }

        public void UpdateActionStance(bool inActionStance)
        {
            if (inActionStance)
            {
                if (InActionStanceCooldown.IsRunning)
                {
                    InActionStanceCooldown.Restart();
                    return;
                }
                InActionStanceCooldown.Start(ActionStanceTime, () => {
                    tno.Send("SetActionStance", Target.AllSaved, false);
                });
            }
            else
            {
                InActionStanceCooldown.Reset();
            }
            tno.Send("SetActionStance", Target.AllSaved, inActionStance);
        }

        [RFC]
        public void SetActionStance(bool inActionStance)
        {
            if (InActionStance != inActionStance)
            {
                InActionStance = inActionStance;
                tno.Send("SetAnimValue", Target.AllSaved, "ActionStance", "BOOL", InActionStance);
            }
        }

        void SwitchWeapon(int weaponType)
        {
            int index = weaponType - 1;
            if (currentWeapon != null)
            {
                currentWeapon.SwitchingOut();
                if (currentWeapon.baseWeapon.myType == (WeaponType)weaponType) { index = -1; } // reclickng the same weapon key unequps the weapon for now
                previousWeapon = GetWeaponByWeaponType(currentWeapon.baseWeapon.myType);
            }

            if (index < 0) // setting weapon to none
            {
                currentWeapon = null;
            }
            else
            {
                SetTool(PlayerTool.None);
                currentWeapon = GetWeaponByWeaponType((WeaponType)weaponType);
            }
            weaponSwitching = true;
            WeaponEquipCooldown.Start(2, () =>
            {
                weaponSwitching = false;
            });
            var wpTypeIndex = currentWeapon != null ? currentWeapon.WeaponClassIndex() : 0;
            Battle.BattlePlayerInput.instance.tno.Send("SetWeaponType", Target.AllSaved, (int)wpTypeIndex);
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
                        if (BattlePlayerInput.instance.GetAxis("Fire1") > 0)
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

            if (ObstacleScripts == null || ObstaclePrefabs.Length != ObstacleScripts.Length)
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
            if (BattlePlayerInput.instance.GetAxis("Fire1") > 0)
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
            if (pickedUp) { /*Debug.LogFormat("picked up {0}", p.name);*/ }
            return pickedUp;
        }

        public void DrawWeapon(int weaponType)
        {
            if (!tno.isMine) return;
//            Debug.Log("drawing Weapon");
            currentWeapon.prefabInstance.tno.Send("Mount", Target.AllSaved, tno.uid, WeaponMountPoints.RightHand);
            tno.Send("SetAnimValue", Target.All, "SwitchingWeapons", "BOOL", false);
            UpdateActionStance(true);
        }

        public void PlaceWeapon(int weaponType)
        {
            if (!tno.isMine) return;
//            Debug.Log("Holstering Weapon");
            if (previousWeapon != null && previousWeapon.prefabInstance != null)
            {
                previousWeapon.prefabInstance.tno.Send("Mount", Target.AllSaved, tno.uid, previousWeapon.prefabInstance.mountPoint);
            }
            if (currentWeapon != null && currentWeapon.WeaponClassIndex() == previousWeapon.WeaponClassIndex())
            {
                currentWeapon.prefabInstance.tno.Send("Mount", Target.AllSaved, tno.uid, WeaponMountPoints.RightHand);
                UpdateActionStance(true);
            }
        }

        public void PlaceGameObject(GameObject mountable, int mountPoint)
        {
            Transform parent = transform;
            switch ((WeaponMountPoints)mountPoint)
            {
                case WeaponMountPoints.Pistol:
                    parent = MountPoints[0];
                    break;
                case WeaponMountPoints.Rifle:
                    parent = MountPoints[1];
                    break;
                case WeaponMountPoints.RightHand:
                    parent = HeldWeaponMountPoint;
                    break;
                default:
                    break;
            }
//            Debug.LogFormat("Setting prefab {0} to parent {1}", mountable.name, parent.name);
            mountable.transform.SetParent(parent);
            mountable.transform.localPosition = Vector3.zero;
            mountable.transform.localRotation = Quaternion.identity;
        }

        private WeaponInstance GetWeaponByWeaponType(WeaponType t)
        {
            //            return weapons.Where(w => w.baseWeapon.myType == t).FirstOrDefault();
            foreach (WeaponInstance w in weapons)
            {
                if (w.baseWeapon.myType == t)
                {
                    return w;
                }
            }
            return null;
        }

        private bool AddWeaponFromPickup(Pickup p)
        {
            var weaponToAdd = GetWeaponByWeaponType(p.baseWeapon.myType);
            if (weaponToAdd != null)
            {
                return false; // can't pick up a weapon type you already have, for now
            }
            weaponToAdd = new WeaponInstance(p.baseWeapon, WeaponFireCooldown, WeaponReloadCooldown, shootOrigin, tno);
            weapons.Add(weaponToAdd);
            var weaponPrefabInstance = p.GetComponent<WeaponPrefab>();
            if (weaponPrefabInstance != null)
            {
                weaponToAdd.SetWeaponPrefab(weaponPrefabInstance);
                weaponToAdd.prefabInstance.tno.Send("Mount", Target.AllSaved, tno.uid, weaponToAdd.prefabInstance.mountPoint);
            }

            weaponToAdd.AddAmmo(p.Value);
            uiValues.WeaponStatus = weaponToAdd.GetStatus();
            return true;
        }

        private bool AddAmmoFromPickup(Pickup p)
        {
            var pickedUp = false;
            var currentWeapon = GetWeaponByWeaponType(p.baseWeapon.myType);
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

        public ImpactTypes GetImpactType() { return ImpactTypes.Player; }

        public void SpawnEffect(Vector3 hit)
        {
            //BattleGameFxManager.instance.
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
                tno.Send("Die", Target.All);
            }
        }
    }
}