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
        private BattlePlayer _battlePlayer;
        private BattleThirdPersonController m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private bool m_Crouch;
        private float m_TurnAmount;

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
            _battlePlayer = GetComponent<BattlePlayer>();
            mRb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }
            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<BattleThirdPersonController>();
        }


        private void Update()
        {
            if (tno.isMine)
            {
                if (!m_Jump)
                {
                    tno.Send("SetJump", Target.AllSaved, CrossPlatformInputManager.GetButtonDown("Jump"));
                }
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            if (tno.isMine)
            {
                // read inputs
                float h = CrossPlatformInputManager.GetAxis("Horizontal");
                float v = CrossPlatformInputManager.GetAxis("Vertical");
                m_Crouch = Input.GetKey(KeyCode.C);
                m_TurnAmount = Input.GetAxis("Mouse X");

                // calculate move direction to pass to character
                if (m_Cam != null)
                {
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
                    tno.Send("SetInputs", Target.OthersSaved, m_Move,m_TurnAmount, m_Crouch);
                    
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
            Debug.LogFormat("{0} setting inputs m:{1}", tno.owner.name, m);
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

        void UpdateCharacter()
        {
            if (!tno.isMine)
            {
                Debug.LogFormat("{0} updating character with inputs m:{1}", tno.owner.name, m_Move);
            }
            // pass all parameters to the character control script
            Vector3 vizOrigin = transform.position + Vector3.up * 1;
            Debug.DrawLine(vizOrigin, vizOrigin + m_Move.normalized, Color.green);
            m_Character.Move(m_Move, m_TurnAmount, m_Crouch, m_Jump);
            m_Jump = false;
        }


        CursorLockMode wantedMode;

        // Apply requested cursor state
        void SetCursorState()
        {
            Cursor.lockState = wantedMode;
            // Hide cursor when locking
            Cursor.visible = (CursorLockMode.Locked != wantedMode);
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();
            // Release cursor on escape keypress
            if (Input.GetKeyDown(KeyCode.Escape))
                Cursor.lockState = wantedMode = CursorLockMode.None;

            switch (Cursor.lockState)
            {
                case CursorLockMode.None:
                    GUILayout.Label("Cursor is normal");
                    if (GUILayout.Button("Lock cursor"))
                        wantedMode = CursorLockMode.Locked;
                    if (GUILayout.Button("Confine cursor"))
                        wantedMode = CursorLockMode.Confined;
                    break;
                case CursorLockMode.Confined:
                    GUILayout.Label("Cursor is confined");
                    if (GUILayout.Button("Lock cursor"))
                        wantedMode = CursorLockMode.Locked;
                    if (GUILayout.Button("Release cursor"))
                        wantedMode = CursorLockMode.None;
                    break;
                case CursorLockMode.Locked:
                    GUILayout.Label("Cursor is locked");
                    if (GUILayout.Button("Unlock cursor"))
                        wantedMode = CursorLockMode.None;
                    if (GUILayout.Button("Confine cursor"))
                        wantedMode = CursorLockMode.Confined;
                    break;
            }

            GUILayout.EndVertical();

            SetCursorState();
        }
    }
}

