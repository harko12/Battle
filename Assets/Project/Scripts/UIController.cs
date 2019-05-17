﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public CanvasGroup hudScreen;
    public CanvasGroup gameOverScreen;
    public GameEvents gameEvents;
    public UIValues myValues;

    [Header("Text Displays")]
    public Text resourceText;
    public Text toolMessage;
    public Text weaponMessage;
    public Text playerHealth;

    [Header("Tools")]
    public UIToolIcon[] Tools;

    private void Start()
    {
        gameOverScreen.alpha = 0;
        hudScreen.alpha = 1;
        onToolChanged(BattlePlayer.PlayerTool.None);
    }

    private void OnEnable()
    {
        gameEvents.OnToolChanged.AddListener(onToolChanged);
        gameEvents.OnPlayerDeath.AddListener(onGameOver);
        gameEvents.OnPlayerSpawn.AddListener(onPlayerSpawn);
    }

    private void OnDisable()
    {
        gameEvents.OnToolChanged.RemoveListener(onToolChanged);
        gameEvents.OnPlayerDeath.RemoveListener(onGameOver);
        gameEvents.OnPlayerSpawn.RemoveListener(onPlayerSpawn);
    }

    public void onGameOver(BattlePlayer p)
    {
        gameOverScreen.alpha = 1;
    }

    public void onPlayerSpawn()
    {
        gameOverScreen.alpha = 0;
    }

    public void onToolChanged(BattlePlayer.PlayerTool tool)
    {
        foreach (var t in Tools)
        {
            bool selected = false;
            if (t.ToolType == tool)
            {
                selected = true;
            }
            t.Selected(selected);
        }
    }

    private void Update()
    {
        resourceText.text = string.Format("Resources: {0}", myValues.ResourceCount);
        toolMessage.text = myValues.ToolMessage;
        weaponMessage.text = myValues.WeaponStatus;

        playerHealth.color = Color.white;
        playerHealth.text = string.Format("Health: {0}", myValues.PlayerHealth);
        if (myValues.PlayerHealth < 10)
        {
            playerHealth.color = Color.red;
        }
    }
}
