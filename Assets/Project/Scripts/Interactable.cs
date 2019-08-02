﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TNet;

namespace Battle
{
    public abstract class Interactable : TNBehaviour, IInteractable
    {
        public virtual void Interact(BattlePlayer p)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool CanInteract(BattlePlayer p)
        {
            return true;
        }
    }
}