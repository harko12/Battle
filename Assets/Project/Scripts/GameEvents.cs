using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Game Event Manager", fileName = "GameEvents")]
public class GameEvents : ScriptableObject
{
    [SerializeField]
    public ToolChangeEvent OnToolChanged;
    public BattlePlayerEvent OnPlayerDeath;
    public UnityEvent OnPlayerSpawn;
}

[System.Serializable]
public class ToolChangeEvent: UnityEvent<BattlePlayer.PlayerTool>
{

}

[System.Serializable]
public class BattlePlayerEvent: UnityEvent<BattlePlayer>
{

}