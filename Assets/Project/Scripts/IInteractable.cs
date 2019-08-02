using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public interface IInteractable
    {
        void Interact(BattlePlayer p);
        bool CanInteract(BattlePlayer p);
    }
}