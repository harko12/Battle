using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class WeaponTests : MonoBehaviour
    {
        public WeaponPrefab[] Weapons;
        public float fireDelay = .5f;
        private float nextFire = 0f;


        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (Time.time < nextFire) return;
                foreach( var w in Weapons)
                {
                    w.tno.SendQuickly("WeaponFireEffects", TNet.Target.All);
                }
                nextFire = Time.time + fireDelay;
            }
        }
    }
}
