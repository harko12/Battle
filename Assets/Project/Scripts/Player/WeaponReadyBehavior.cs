using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReadyBehavior : StateMachineBehaviour
{
    private BattlePlayer player;
    public bool AlwaysFalse;

    private void getPlayer(GameObject go)
    {
        if (player == null)
        {
            player = go.GetComponent<BattlePlayer>();
        }
    }
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        getPlayer(animator.gameObject);
        var inActionStance = animator.GetBool("ActionStance");
        player.weaponReady = AlwaysFalse ? false : inActionStance;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        getPlayer(animator.gameObject);
        var inActionStance = animator.GetBool("ActionStance");
        player.weaponReady = AlwaysFalse ? false : inActionStance;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        getPlayer(animator.gameObject);
        player.weaponReady = false;
    }
}
