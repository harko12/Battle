using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattlePlayerAnimManager : MonoBehaviour
    {
        BattlePlayerInput bpInput;
        BattlePlayer bPlayer;

        private void Awake()
        {
            bpInput = BattlePlayerInput.instance;
            bPlayer = BattlePlayer.instance;
        }


    }
}