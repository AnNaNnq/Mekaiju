using UnityEngine;
using UnityEngine.UI;

namespace Mekaiju.UI
{
    public class AudioTabManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public Slider musicVolumeSlider;
        public Toggle musicEnableToggle;
        public Slider sfxVolumeSlider;
        public Toggle sfxEnableToggle;

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;

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
            musicSource.volume = p_volume;
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, p_volume);
        }

        // Enable or disable music
        public void ToggleMusic(bool p_isEnabled)
        {
            musicSource.mute = !p_isEnabled;
            PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, p_isEnabled ? 1 : 0);
        }

        // Set SFX volume and save it
        public void SetSfxVolume(float p_volume)
        {
            sfxSource.volume = p_volume;
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, p_volume);
        }

        // Enable or disable SFX
        public void ToggleSfx(bool p_isEnabled)
        {
            sfxSource.mute = !p_isEnabled;
            PlayerPrefs.SetInt(SFX_ENABLED_KEY, p_isEnabled ? 1 : 0);
        }

        // Load saved settings
        private void _LoadSettings()
        {
            float t_musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            musicSource.volume = t_musicVolume;
            musicVolumeSlider.value = t_musicVolume;

            bool t_musicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
            musicSource.mute = !t_musicEnabled;
            musicEnableToggle.isOn = t_musicEnabled;

            float t_sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
            sfxSource.volume = t_sfxVolume;
            sfxVolumeSlider.value = t_sfxVolume;

            bool t_sfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1;
            sfxSource.mute = !t_sfxEnabled;
            sfxEnableToggle.isOn = t_sfxEnabled;
        }
    }
}
