using HarkoGames.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

namespace Battle
{
    public enum ImpactTypes { Metal, Concrete, Wood, Dirt, Player };

    public class BattleGameFxManager : SingletonMonoBehaviour<BattleGameFxManager>
    {
        public ObjectPoolPO[] ImpactEffectPools;
        public TNObject tno;
        public float PurgeTime = 1f;
        private float lastPurge;
        private Queue<GameObject> activeEffects;

        protected override void Awake()
        {
            base.Awake();
            tno = GetComponent<TNObject>();
        }

        public void SpawnImpact(ImpactTypes t, Vector3 pos, Quaternion rot)
        {
            tno.SendQuickly("SpawnEffect", Target.All, t, pos, rot);
        }

        [RFC]
        public void SpawnEffect(ImpactTypes t, Vector3 pos, Quaternion rot)
        {
            var pool = ImpactEffectPools[(int)t];
            var effect = pool.Get();
            effect.myPool = pool;
            effect.transform.position = pos;
            effect.transform.rotation = rot;
            effect.gameObject.SetActive(true);
            var sound = effect.GetComponent<AudioSource>();
            if (sound != null)
            {
                sound.Play();
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}