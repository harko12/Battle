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
        public GameEvents gameEvents;
        public BattleGameSettings settings;

        private void Start()
        {
            SetVolumeLevels();
        }

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            gameEvents.OnSettingsUpdated.AddListener(UpdateSettings);
        }

        private void OnDisable()
        {
            gameEvents.OnSettingsUpdated.RemoveListener(UpdateSettings);
        }

        public void PlayThemeMusic()
        {
            MusicSource.clip = battleMusic.ThemeMusic;
            MusicSource.PlayDelayed(.5f);
        }

        public void UpdateSettings()
        {
            SetVolumeLevels();
        }

        public void SetVolumeLevels()
        {
            audioMixer.SetFloat("masterVolume", settings.MasterVolume);
            audioMixer.SetFloat("soundFxVolume", settings.SoundVolume);
            audioMixer.SetFloat("musicVolume", settings.MusicVolume);
        }
    }
}
