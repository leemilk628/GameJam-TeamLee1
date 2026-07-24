using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Key.Scripts.Singletone {
    public enum SoundType {
        MainBGM,
        GameBGM,

        ButtonClick,
        PlayerShoot,
        AutoCannonSatelliteShoot,
        MissileSatelliteShoot,
        LaserSatelliteShoot,
        PlayerHit,
        EnemyHit,
        Explosion,
        Upgrade,
        NotEnoughMoney,
        SkillTreeUnlock,
        Conversion,
        GameOver
    }

    [Serializable]
    public class SoundData {
        public SoundType soundType;
        public AudioClip audioClip;

        [Range(0f, 1f)]
        public float volume = 1f;
    }

    public class SoundManager : MonoBehaviour {
        public static SoundManager Instance { get; private set; }

        public float BGMVolume { get; private set; } = 1f;
        public float SFXVolume { get; private set; } = 1f;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Audio Source")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Sound Data")]
        [SerializeField] private SoundData[] soundDataList;

        private const string BGMVolumeParameter = "BGMVolume";
        private const string SFXVolumeParameter = "SFXVolume";

        private const string BGMVolumeSaveKey = "BGM_VOLUME";
        private const string SFXVolumeSaveKey = "SFX_VOLUME";

        private readonly Dictionary<SoundType, SoundData>
            _soundDictionary = new();

        private SoundType? _currentBGM;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Start() {
            LoadVolume();
        }

        private void Initialize() {
            _soundDictionary.Clear();

            if (soundDataList != null) {
                foreach (SoundData soundData in soundDataList) {
                    if (soundData == null ||
                        soundData.audioClip == null) {
                        continue;
                    }

                    if (_soundDictionary.ContainsKey(soundData.soundType)) {
                        continue;
                    } //중복일때 스킵

                    _soundDictionary.Add(
                        soundData.soundType,
                        soundData
                    );
                }
            }

            if (bgmSource != null) {
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
            }

            if (sfxSource != null) {
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
        }

        public void PlayBGM(SoundType soundType) {
            if (bgmSource == null)
                return;

            if (!_soundDictionary.TryGetValue(
                    soundType,
                    out SoundData soundData
                )) {
                Debug.LogWarning(
                    $"{name}: {soundType} 사운드를 찾을 수 없음",
                    this
                );

                return;
            }

            if (_currentBGM == soundType &&
                bgmSource.isPlaying) {
                return;
            }

            _currentBGM = soundType;

            bgmSource.clip = soundData.audioClip;
            bgmSource.volume = soundData.volume;
            bgmSource.Play();
        }

        public void StopBGM() {
            if (bgmSource == null)
                return;

            bgmSource.Stop();
            bgmSource.clip = null;

            _currentBGM = null;
        }

        public void PlaySFX(SoundType soundType) {
            if (sfxSource == null)
                return;

            if (!_soundDictionary.TryGetValue(
                    soundType,
                    out SoundData soundData
                )) {
                Debug.LogWarning(
                    $"{name}: {soundType} 사운드를 찾을 수 없음",
                    this
                );

                return;
            }

            sfxSource.PlayOneShot(
                soundData.audioClip,
                soundData.volume
            );
        }

        public void SetBGMVolume(float volume) {
            BGMVolume = Mathf.Clamp01(volume);

            SetMixerVolume(
                BGMVolumeParameter,
                BGMVolume
            );
        }

        public void SetSFXVolume(float volume) {
            SFXVolume = Mathf.Clamp01(volume);

            SetMixerVolume(
                SFXVolumeParameter,
                SFXVolume
            );
        }

        private void SetMixerVolume(
            string parameterName,
            float volume
        ) {
            if (audioMixer == null)
                return;

            float decibel = volume <= 0.0001f
                ? -80f
                : Mathf.Log10(volume) * 20f;

            bool result = audioMixer.SetFloat(
                parameterName,
                decibel
            );

            if (!result) {
                Debug.LogWarning(
                    $"{name}: AudioMixer 파라미터 " +
                    $"{parameterName}을 찾을 수 없습니다.",
                    this
                );
            }
        }

        private void LoadVolume() {
            BGMVolume = PlayerPrefs.GetFloat(
                BGMVolumeSaveKey,
                1f
            );

            SFXVolume = PlayerPrefs.GetFloat(
                SFXVolumeSaveKey,
                1f
            );

            SetBGMVolume(BGMVolume);
            SetSFXVolume(SFXVolume);
        }

        public void SaveVolume() {
            PlayerPrefs.SetFloat(
                BGMVolumeSaveKey,
                BGMVolume
            );

            PlayerPrefs.SetFloat(
                SFXVolumeSaveKey,
                SFXVolume
            );

            PlayerPrefs.Save();
        }

        private void OnApplicationQuit() {
            SaveVolume();
        }

        private void OnDestroy() {
            if (Instance == this)
                Instance = null;
        }
    }
}