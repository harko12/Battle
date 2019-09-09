using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Battle
{
    public class BattleGameCameraManager : SingletonMonoBehaviour<BattleGameCameraManager>
    {
        public CinemachineBrain MainBrain;

        public CinemachineFreeLook CharacterFreeLook;
        public Transform FreeLookDefaultTarget;

        public CinemachineVirtualCamera OrbitCam;
        public CinemachineVirtualCamera AimCam;
        public CinemachineVirtualCamera FollowCam;

        private void Start()
        {
            OrbitCam.Priority = 15;
            if (CharacterFreeLook != null)
            {
                CharacterFreeLook.Priority = 0;
            }
        }

        public void PlayerSpawned(BattlePlayer p)
        {
            if (!p.tno.isMine)
            {
                return;
            }
            OrbitCam.Priority = 5;
            p.ToggleAim(false);
        }

        public void SetFreeLookTarget(BattlePlayer p)
        {
            if (!p.tno.isMine)
            {
                return;
            }
            OrbitCam.Priority = 5;
            /*
            CharacterFreeLook.m_Follow = p.transform;
            CharacterFreeLook.m_LookAt = p.rotationPoint.transform;
            CharacterFreeLook.Priority = 10;
            */
        }
    }
}