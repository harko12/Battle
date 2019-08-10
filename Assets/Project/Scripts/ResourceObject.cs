using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

namespace Battle
{

    public class ResourceObject : Interactable, IInteractable, IDamagable
    {
        [SerializeField]
        private int resourceAmount;
        [SerializeField]
        private int hitsToResource = 2;
        private int hits = 0;
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

        private void Start()
        {
            _health = resourceAmount;
        }

        public override void Interact(BattlePlayer p)
        {
            hits++;
            Health--;
            tno.Send("Sync", Target.OthersSaved, hits, Health);
            if (hits == hitsToResource || Health == 0)
            {
                p.AddResource(hits);
                hits = 0;
            }
            CheckDamage();
            StartCoroutine(DamageReactions.QuickScale(transform, 3));
        }

        public override bool CanInteract(BattlePlayer p)
        {
            if (p.Tool == BattlePlayer.PlayerTool.Pickaxe)
            {
                return true;
            }
            return false;
        }

        public void TakeDamage(float damageAmount)
        {
            tno.Send("ApplyDamage", Target.AllSaved, damageAmount);
        }

        public ImpactTypes GetImpactType() { return ImpactTypes.Wood; }

        [RFC]
        public void Sync(int newHits, float newHealth)
        {
            hits = newHits;
            Health = newHealth;
        }

        [RFC]
        public void ApplyDamage(float damageAmount)
        {
            // for now, one shot destroys resources, and gives you nothing
            Health = 0;
            CheckDamage();
            StartCoroutine(DamageReactions.QuickScale(transform, 3));
        }


        private void CheckDamage()
        {
            if (Health <= 0)
            {
                StopAllCoroutines();
                tno.Send("Die", Target.AllSaved);
            }
        }
        [RFC]
        public void Die()
        {
            this.transform.localScale = Vector3.zero;
            DestroySelf();
            //        Destroy(this.gameObject, .5f);
        }
    }
}