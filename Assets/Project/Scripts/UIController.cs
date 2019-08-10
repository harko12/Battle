using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Battle
{

    public class UIController : SingletonMonoBehaviour<UIController>
    {
        public CanvasGroup playerMenu;
        public CanvasGroup hudScreen;
        public CanvasGroup gameOverScreen;
        private GameEvents gameEvents;
        public RectTransform crossHair;
        public UIValues myValues;

        [Header("Text Displays")]
        public Text resourceText;
        public Text toolMessage;
        public Text weaponMessage;
        public Text playerHealth;

        [Header("Tools")]
        public UIToolIcon[] Tools;

        [Header("Menu Panel Objects")]
        public SlideMenu MainMenuPanel;
        public List<SlideMenu> MenuPanels;

        private Canvas myCanvas;

        protected override void Awake()
        {
            base.Awake();
            myCanvas = GetComponent<Canvas>();
            gameEvents = BattleGameObjects.instance.gameEvents;
        }

        private void Start()
        {
            playerMenu.alpha = 0;
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

        public void Disconnect()
        {
            gameEvents.OnPlayerDisconnect.Invoke();
        }

        public void onGameOver(BattlePlayer p)
        {
            hudScreen.alpha = 0;
            gameOverScreen.alpha = 0;
        }

        public void onPlayerSpawn()
        {
            hudScreen.alpha = 1;
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
        public void ToggleMenu()
        {
            var newAlpha = 1;
            if (playerMenu.alpha == 1) { newAlpha = 0; }
            playerMenu.alpha = newAlpha;
            var showing = newAlpha == 1;
            gameEvents.OnMouseReleased.Invoke(showing); // 
            CloseAllMenuPanels();
            if (showing)
            {
                TogglePanel(MainMenuPanel);
            }
        }

        public void TogglePanel(SlideMenu mPanel)
        {
            CloseAllMenuPanels();
            mPanel.Toggle();
        }

        private void CloseAllMenuPanels()
        {
            foreach (var mp in MenuPanels)
            {
                if (mp.IsShowing)
                {
                    mp.Toggle();
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                ToggleMenu();
            }
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
}