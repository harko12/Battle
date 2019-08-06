﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
namespace Battle
{
    public class WeaponPrefab : TNBehaviour
    {
        public WeaponMountPoints mountPoint;
        public Transform ShootOrigin;

        [RFC]
        public void Mount(uint battlePlayerID, int mountPoint)
        {
            var battlePlayerTno = TNObject.Find(tno.channelID, battlePlayerID);
            if (battlePlayerTno == null)
            {
                Debug.LogFormat("unable to find prefab ID {0}", battlePlayerID);
                return;
            }
            var bp = battlePlayerTno.GetComponent<BattlePlayer>();
            bp.PlaceGameObject(gameObject, mountPoint);
            /*
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
            */
        }
    }
}
