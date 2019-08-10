using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Battle
{
    public class BattleGameMusicManager : SingletonMonoBehaviour<BattleGameMusicManager>
    {
        public AudioMixer audioMixer;
        public AudioSource MusicSource;
        public BattleGameMusic battleMusic;
        private GameEvents gameEvents;
        private BattleGameSettings settings;

        private void Start()
        {
            SetVolumeLevels();
        }
        
        protected override void Awake()
        {
            base.Awake();
            gameEvents = BattleGameObjects.instance.gameEvents;
            settings = BattleGameObjects.instance.settings;
        }

        private void OnEnable()
        {
            gameEvents.OnPlayerSpawn.AddListener(onPlayerSpawn);
            gameEvents.OnSettingsUpdated.AddListener(UpdateSettings);
        }

        private void OnDisable()
        {
            gameEvents.OnPlayerSpawn.RemoveListener(onPlayerSpawn);
            gameEvents.OnSettingsUpdated.RemoveListener(UpdateSettings);
        }

        public void onPlayerSpawn()
        {
            if (!MusicSource.isPlaying)
            {
                PlayThemeMusic();
            }
        }

        public void PlayThemeMusic()
        {
            MusicSource.clip = battleMusic.ThemeMusic;
            MusicSource.PlayDelayed(.5f);
        }

        public void UpdateSettings()
        {
            Debug.Log("setting volume levels");
            SetVolumeLevels();
        }

        public void SetVolumeLevels()
        {
            if (audioMixer == null) { Debug.Log("audo mixer is null"); return; }
            if (settings == null) { Debug.Log("settings object is null"); return; }
            Debug.Log("setting volume levels");
            audioMixer.SetFloat("masterVolume", settings.MasterVolume);
            audioMixer.SetFloat("soundFxVolume", settings.SoundVolume);
            audioMixer.SetFloat("musicVolume", settings.MusicVolume);
        }
    }
}
