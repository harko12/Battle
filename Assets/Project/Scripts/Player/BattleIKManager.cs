using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleIKManager : MonoBehaviour
    {
        [SerializeField]
        private RootMotion.FinalIK.AimIK aimIk;
        [SerializeField]
        private RootMotion.FinalIK.LookAtIK lookIk;
        /*
        public void UpdateIK(WeaponInstance currentWeapon, bool isAiming)
        {
            var lookWeight = 0;
            var aimWeight = currentWeapon != null && InActionStance && currentWeapon.WeaponTypeIndex() > 0 ? 1 : 0;
            aimIk.solver.IKPositionWeight = aimWeight;
            lookIk.solver.IKPositionWeight = lookWeight;
        }
        */
    }
}