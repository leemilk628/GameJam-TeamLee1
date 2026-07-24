using Key.Scripts.Singletone;
using UnityEngine;
using UnityEngine.UI;

namespace Key.Scripts.Sound {
    public class SoundSettingUI : MonoBehaviour {
        [Header("Volume Slider")]
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        private void Start() {
            if (SoundManager.Instance == null) {
                Debug.LogError(
                    $"{name}: SoundManager가 존재하지 않습니다.",
                    this
                );

                return;
            }

            InitializeSlider();
            RegisterSliderEvent();
        }

        private void InitializeSlider() {
            if (bgmSlider != null) {
                bgmSlider.minValue = 0f;
                bgmSlider.maxValue = 1f;
                bgmSlider.wholeNumbers = false;

                bgmSlider.SetValueWithoutNotify(
                    SoundManager.Instance.BGMVolume
                );
            }

            if (sfxSlider != null) {
                sfxSlider.minValue = 0f;
                sfxSlider.maxValue = 1f;
                sfxSlider.wholeNumbers = false;

                sfxSlider.SetValueWithoutNotify(
                    SoundManager.Instance.SFXVolume
                );
            }
        }

        private void RegisterSliderEvent() {
            if (bgmSlider != null)
                bgmSlider.onValueChanged.AddListener(
                    OnBGMVolumeChanged
                );

            if (sfxSlider != null)
                sfxSlider.onValueChanged.AddListener(
                    OnSFXVolumeChanged
                );
        }

        private void OnBGMVolumeChanged(float volume) {
            SoundManager.Instance?.SetBGMVolume(volume);
        }

        private void OnSFXVolumeChanged(float volume) {
            SoundManager.Instance?.SetSFXVolume(volume);
        }

        private void OnDisable() {
            SoundManager.Instance?.SaveVolume();
        }

        private void OnDestroy() {
            if (bgmSlider != null)
                bgmSlider.onValueChanged.RemoveListener(
                    OnBGMVolumeChanged
                );

            if (sfxSlider != null)
                sfxSlider.onValueChanged.RemoveListener(
                    OnSFXVolumeChanged
                );
        }
    }
}