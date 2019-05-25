using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

public class Obstacle : TNBehaviour, IDamagable
{
    public int Cost;
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
    [SerializeField]
    private float _health;
    private float targetRotation;
    private float reactionTime = .1f;

    public void TakeDamage(float damageAmount)
    {
        tno.Send("ApplyDamage", Target.AllSaved, damageAmount);
    }

    [RFC]
    public void ApplyDamage(float damageAmount)
    {
        Health -= damageAmount;
        if (Health <= 0)
        {
            Die();
            return;
        }
        StartCoroutine(DamageReactions.ShiftPosition(transform, .1f));
    }

    public void Die()
    {
        this.transform.localScale = Vector3.zero;
    }

    [RCC]
    static GameObject CreateObstacle(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        // Instantiate the prefab
        GameObject go = prefab.Instantiate();

        // Set the position and rotation based on the passed values
        Transform t = go.transform;
        t.position = pos;
        t.rotation = rot;

        return go;
    }

}
