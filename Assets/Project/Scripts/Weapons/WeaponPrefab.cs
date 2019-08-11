using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using RootMotion.FinalIK;

namespace Battle
{
    public class WeaponPrefab : TNBehaviour
    {
        public WeaponMountPoints mountPoint;
        public Transform ShootOrigin;
        public ParticleSystem MuzzleFlash;
        public float MuzzleFlashTime = .2f;
        public AudioSource ShootSound;

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
        }

        [RFC]
        public void WeaponFireEffects()
        {
            ShowMuzzleFlash();
            PlayFireSound();
        }

        private void PlayFireSound()
        {
            if (ShootSound != null)
            {
                ShootSound.Play();
            }
        }

        private void ShowMuzzleFlash()
        {
            if (MuzzleFlash != null)
            {
                MuzzleFlash.Play(true);
            }
        }
    }
}
