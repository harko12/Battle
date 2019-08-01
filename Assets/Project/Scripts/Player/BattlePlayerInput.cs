﻿using Cinemachine;
using HarkoGames.Utilities;
using System.Collections;
using System.Collections.Generic;
using TNet;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Battle
{
    [RequireComponent(typeof(BattlePlayer))]
    [RequireComponent(typeof(BattleThirdPersonController))]
    public class BattlePlayerInput : TNBehaviour
    {
        public static BattlePlayerInput instance;
        public BattleGameSettings Settings;
        private BattlePlayer _battlePlayer;
        private BattleThirdPersonController m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        [SerializeField]
        private bool m_Aim;
        private bool m_Crouch;
        public float m_TurnAmount { get; private set; }
        public float VerticalAngle { get; private set; }

        [Range(1f, 20f)]
        public float inputUpdates = 10f;
        protected float mLastInputSend = 0f;
        [Range(0.25f, 5f)]
        public float rigidbodyUpdates = 1f;
        protected float mNextRB = 0f;
        private Rigidbody mRb;
        protected Vector3 mLastMove;

        protected override void Awake()
        {
            base.Awake();
            if (tno.isMine)
            {
                instance = this;
            }
            _battlePlayer = GetComponent<BattlePlayer>();
            mRb = GetComponent<Rigidbody>();
        }

        private CinemachineBrain GetCmBrain()
        {
            if (CinemachineCore.Instance.BrainCount > 0)
            {
                return CinemachineCore.Instance.GetActiveBrain(0);
            }
            return null;

        }

        CursorLockMode wantedMode;

        // Apply requested cursor state
        void SetCursorState()
        {
            Cursor.lockState = wantedMode;
            // Hide cursor when locking
            Cursor.visible = (CursorLockMode.Locked != wantedMode);
        }

        private bool mInputSuspended;
        public void SuspendInputs(bool suspend)
        {
            mInputSuspended = suspend;
            wantedMode = suspend ? CursorLockMode.None : CursorLockMode.Locked;
            SetCursorState();
            if (suspend)
            {
                m_TurnAmount = 0;
                VerticalAngle = 0;
                m_Move = Vector3.zero;
                tno.Send("SetInputs", Target.OthersSaved, m_Move, m_TurnAmount, m_Crouch);
            }
        }

        public float GetAxis(string axis)
        {
            if (mInputSuspended)
            {
                return 0;
            }
            return Input.GetAxis(axis);
        }

        public bool GetKey(KeyCode code)
        {
            if (mInputSuspended)
            {
                return false;
            }
            return Input.GetKey(code);
        }

        private void Start()
        {
            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<BattleThirdPersonController>();
            wantedMode = CursorLockMode.Locked;
            SetCursorState();
            Cinemachine.CinemachineCore.GetInputAxis = GetAxisCustom;
        }

        public float GetAxisCustom(string axisName)
        {
            if (axisName == "Mouse X")
            {
                //return 0;
                return UnityEngine.Input.GetAxis("Mouse X");
            }
            else if (axisName == "Mouse Y")
            {
                var multiplier = 1;
                if (BattlePlayerInput.instance.Settings.InvertMouse)
                {
                    multiplier = -1;
                }
                return UnityEngine.Input.GetAxis("Mouse Y") * multiplier;
            }
            return UnityEngine.Input.GetAxis(axisName);
        }

        private void Update()
        {
            if (tno.isMine && !mInputSuspended)
            {
                m_Cam = null;
                var cmBrain = GetCmBrain();
                if (cmBrain != null)
                {
                    var blah = cmBrain.ActiveVirtualCamera;
                    if (blah != null)
                    {
                        m_Cam = blah.VirtualCameraGameObject.transform;
                    }
                }

                if (!m_Jump)
                {
                    tno.Send("SetJump", Target.AllSaved, CrossPlatformInputManager.GetButtonDown("Jump"));
                }
            }
        }

        public float MaxFreelookFacingDiff = .1f;
        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (tno.isMine && !mInputSuspended)
            {
                // read inputs
                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");
                var inputVector = new Vector3(h, 0, v);
                m_Crouch = Input.GetKey(KeyCode.C);
                
                var new_aim = Input.GetAxis("Fire2") > .01f;
                if (m_Aim != new_aim)
                {
                    _battlePlayer.tno.Send("ToggleAim", Target.AllSaved, new_aim);
                }
                m_Aim = new_aim;
                
                //_battlePlayer.ToggleAim(m_Aim);
                m_TurnAmount =  Input.GetAxis("Mouse X");
                VerticalAngle = Input.GetAxis("Mouse Y");
                // calculate move direction to pass to character
                if (m_Cam != null)
                {
//                    Debug.Log("setting move from camera perspective");
                    // calculate camera relative direction to move:
                    m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                    m_Move = v * m_CamForward + h * m_Cam.right;
                }
                else
                {
                    // we use world-relative directions in the case of no main camera
                    m_Move = v * Vector3.forward + h * Vector3.right;
                }
#if !MOBILE_INPUT
                // walk speed multiplier
                if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif


                // Objects get marked as destroyed while being transferred from one channel to another
                if (tno.hasBeenDestroyed) return;

                float time = Time.time;
                float delta = time - mLastInputSend;
                float delay = 1f / inputUpdates;

                // Don't send updates more than 20 times per second
                if (delta > 0.05f)
                {
                    /*
                    // The closer we are to the desired send time, the smaller is the deviation required to send an update.
                    float threshold = Mathf.Clamp01(delta - delay) * 0.5f;

                    // If the deviation is significant enough, send the update to other players
                    if (Tools.IsNotEqual(mLastMove.x, m_Move.x, threshold) ||
                        Tools.IsNotEqual(mLastMove.y, m_Move.y, threshold))
                    {
                        mLastInputSend = time;
                        mLastMove = m_Move;
                        tno.Send("SetInputs", Target.OthersSaved, m_Move, m_Jump, m_Crouch);
                    }
                    */
                    mLastInputSend = time;
                    tno.Send("SetInputs", Target.OthersSaved, m_Move, m_TurnAmount, m_Crouch);
                    
                    // Since the input is sent frequently, rigidbody only needs to be corrected every couple of seconds.
                    // Faster-paced games will require more frequent updates.
                    if (mNextRB < time)
                    {
                        mNextRB = time + 1f / rigidbodyUpdates;
                        tno.Send("SetRB", Target.OthersSaved, mRb.position, mRb.rotation, mRb.velocity, mRb.angularVelocity);
                    }
                }
            }
            UpdateCharacter();
        }

        [RFC]
        protected void SetJump(bool j)
        {
            m_Jump = j;
        }

        [RFC]
        protected void SetInputs(Vector3 m, float t, bool c) {
            //Debug.LogFormat("{0} setting inputs m:{1}", tno.owner.name, m);
            m_Move = m; m_TurnAmount = t;  m_Crouch = c;
        }

        /// <summary>
        /// RFC for the rigidbody will be called once per second by default.
        /// </summary>

        [RFC]
        protected void SetRB(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
        {
            mRb.position = pos;
            mRb.rotation = rot;
            mRb.velocity = vel;
            mRb.angularVelocity = angVel;
        }

        public void NewEvent()
        {

        }
        public void WeaponHolstered()
        {

        }

        void UpdateCharacter()
        {
            if (!tno.isMine)
            {
                //Debug.LogFormat("{0} updating character with inputs m:{1}", tno.owner.name, m_Move);
            }
            // pass all parameters to the character control script
            Vector3 vizOrigin = transform.position + Vector3.up * 1;
            Debug.DrawLine(vizOrigin, vizOrigin + m_Move.normalized, Color.green);
            m_Character.SetAnimValue("Aiming", "BOOL", m_Aim);
            m_Character.Move(m_Move, m_TurnAmount, m_Crouch, m_Jump);
            m_Jump = false;
        }

        [RFC]
        public void SetWeapon(int weaponIndex) //needs to be tnet serializable
        {
            var index = 0;
            float weight = 1;
            if (weaponIndex > 0)
            {
                index = weaponIndex;
                weight = 1;
            }
            m_Character.SetAnimValue("WeaponType", "INT", index);
            m_Character.SetAnimValue("SwitchWeapon", "TRIGGER");
            m_Character.SetAnimLayerWeight("Upper Body", weight);
        }

        [RFC]
        public void SetAnimTrigger(string triggerName)
        {
            m_Character.SetAnimValue(triggerName, "TRIGGER");
        }

        [RFC]
        public void SetTimedTrigger(string triggerName, string timeParm, float t)
        {
            m_Character.SetAnimValue(triggerName, "TRIGGER");
            m_Character.SetAnimValue(timeParm, "FLOAT", t);
        }
    }
}

