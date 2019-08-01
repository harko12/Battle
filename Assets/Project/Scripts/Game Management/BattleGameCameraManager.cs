﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BattleGameCameraManager : SingletonMonoBehaviour<BattleGameCameraManager>
{
    public CinemachineBrain MainBrain;

    public CinemachineFreeLook CharacterFreeLook;
    public Transform FreeLookDefaultTarget;

    public CinemachineVirtualCamera OrbitCam;

    private void Start()
    {
        OrbitCam.Priority = 15;
        CharacterFreeLook.Priority = 0;
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
