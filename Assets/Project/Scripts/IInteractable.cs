using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(BattlePlayer p);
    bool CanInteract(BattlePlayer p);
}
