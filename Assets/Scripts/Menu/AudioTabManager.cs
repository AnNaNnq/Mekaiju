using UnityEngine;
using UnityEngine.UI;

namespace Mekaiju.Audio
{
    public class AudioTabManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public Slider MusicVolumeSlider;
        public Toggle MusicEnableToggle;
        public Slider SfxVolumeSlider;
        public Toggle SfxEnableToggle;

        [Header("Audio Sources")]
        public AudioSource MusicSource;
        public AudioSource SfxSource;

        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const string MUSIC_ENABLED_KEY = "MusicEnabled";
        private const string SFX_VOLUME_KEY = "SFXVolume";
        private const string SFX_ENABLED_KEY = "SFXEnabled";

        void Start()
        {
            _LoadSettings();
        }

        // Set music volume and save it
        public void SetMusicVolume(float p_volume)
        {
            MusicSource.volume = p_volume;
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, p_volume);
        }

        // Enable or disable music
        public void ToggleMusic(bool p_isEnabled)
        {
            MusicSource.mute = !p_isEnabled;
            PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, p_isEnabled ? 1 : 0);
        }

        // Set SFX volume and save it
        public void SetSfxVolume(float p_volume)
        {
            SfxSource.volume = p_volume;
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, p_volume);
        }

        // Enable or disable SFX
        public void ToggleSfx(bool p_isEnabled)
        {
            SfxSource.mute = !p_isEnabled;
            PlayerPrefs.SetInt(SFX_ENABLED_KEY, p_isEnabled ? 1 : 0);
        }

        // Load saved settings
        private void _LoadSettings()
        {
            float t_musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            MusicSource.volume = t_musicVolume;
            MusicVolumeSlider.value = t_musicVolume;

            bool t_musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
            MusicSource.mute = !t_musicEnabled;
            MusicEnableToggle.isOn = t_musicEnabled;

            float t_sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            SfxSource.volume = t_sfxVolume;
            SfxVolumeSlider.value = t_sfxVolume;

            bool t_sfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1;
            SfxSource.mute = !t_sfxEnabled;
            SfxEnableToggle.isOn = t_sfxEnabled;
        }
    }
}
