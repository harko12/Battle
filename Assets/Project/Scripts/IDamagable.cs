﻿using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    float Health { get; set; }
    void TakeDamage(float damageAmount);
    void Die();
    ImpactTypes GetImpactType();
}
