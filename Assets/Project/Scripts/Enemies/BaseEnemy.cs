using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;
using Battle;

public class BaseEnemy : TNBehaviour, IDamagable
{
    [SerializeField]
    private float _health;
    protected Rigidbody myRigidbody;
    public float Damage;

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

    protected override void Awake()
    {
        base.Awake();
        myRigidbody = GetComponent<Rigidbody>();
    }

    public void Die()
    {
        this.transform.localScale = Vector3.zero;
        this.gameObject.SetActive(false);
    }

    public void TakeDamage(float damageAmount)
    {
        tno.Send("ApplyDamage", Target.AllSaved, damageAmount);
    }

    public ImpactTypes GetImpactType() { return ImpactTypes.Metal; }

    [RFC]
    public void ApplyDamage(float damageAmount)
    {
        Health -= damageAmount;
        //Debug.Log("Enemy damaged for " + damageAmount.ToString());
        if (Health <= 0)
        {
            Die();
            return;
        }
        StartCoroutine(DamageReactions.ShiftPosition(transform, .1f));
    }
}
