﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Battle;

[CreateAssetMenu(menuName = "Game Event Manager", fileName = "GameEvents")]
public class GameEvents : ScriptableObject
{
    [SerializeField]
    public ToolChangeEvent OnToolChanged;
    public BattlePlayerEvent OnPlayerDeath;
    public UnityEvent OnPlayerSpawn;
    public UnityEvent OnPlayerDisconnect;
    public IntEvent OnChangeHost;
    public BoolEvent OnMouseReleased;
    public UnityEvent OnSettingsUpdated;
}

[System.Serializable]
public class ToolChangeEvent: UnityEvent<BattlePlayer.PlayerTool>
{

}

[System.Serializable]
public class BattlePlayerEvent: UnityEvent<BattlePlayer>
{

}

[System.Serializable]
public class IntEvent : UnityEvent<int>
{

}

[System.Serializable]
public class BoolEvent : UnityEvent<bool>
{

}

[System.Serializable]
public class Vector2Event : UnityEvent<Vector2>
{

}
