using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Mekaiju.Graphics
{
    public class GraphicsTabManager : MonoBehaviour
    {
        // UI elements
        public TMP_Dropdown ResolutionDropdown;  // Dropdown for screen resolution selection
        public TMP_Dropdown QualityDropdown;     // Dropdown for graphics quality selection
        public Toggle ReflectionsToggle;     // Toggle for real-time reflections
        public Toggle VSyncToggle;           // Toggle for V-Sync
        public Toggle FullscreenToggle;      // Toggle for fullscreen mode

        private Resolution[] _resolutions;   // Available screen resolutions

        void Start()
        {
            // Load available screen resolutions
            _resolutions = Screen.resolutions;
            ResolutionDropdown.ClearOptions();
            List<string> t_options = new List<string>();

            int t_currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string t_option = _resolutions[i].width + " x " + _resolutions[i].height;
                t_options.Add(t_option);

                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    t_currentResolutionIndex = i;
                }
            }

            ResolutionDropdown.AddOptions(t_options);
            ResolutionDropdown.value = t_currentResolutionIndex;
            ResolutionDropdown.RefreshShownValue();

            // Load quality settings
            QualityDropdown.ClearOptions();
            QualityDropdown.AddOptions(new List<string>(QualitySettings.names));
            QualityDropdown.value = QualitySettings.GetQualityLevel();
            QualityDropdown.RefreshShownValue();

            // Load toggle settings
            ReflectionsToggle.isOn = PlayerPrefs.GetInt("Reflections", 1) == 1;
            VSyncToggle.isOn = QualitySettings.vSyncCount > 0;
            FullscreenToggle.isOn = Screen.fullScreen;

            // Add event listeners
            ResolutionDropdown.onValueChanged.AddListener(p_index => SetResolution(p_index));
            QualityDropdown.onValueChanged.AddListener(p_index => SetQuality(p_index));
            ReflectionsToggle.onValueChanged.AddListener(p_enabled => SetReflections(p_enabled));
            VSyncToggle.onValueChanged.AddListener(p_enabled => SetVSync(p_enabled));
            FullscreenToggle.onValueChanged.AddListener(p_enabled => SetFullscreen(p_enabled));
        }

        private void SetResolution(int p_index)
        {
            Screen.SetResolution(_resolutions[p_index].width, _resolutions[p_index].height, Screen.fullScreen);
        }

        private void SetQuality(int p_index)
        {
            QualitySettings.SetQualityLevel(p_index);
        }

        private void SetReflections(bool p_enabled)
        {
            PlayerPrefs.SetInt("Reflections", p_enabled ? 1 : 0);
        }

        private void SetVSync(bool p_enabled)
        {
            QualitySettings.vSyncCount = p_enabled ? 1 : 0;
        }

        private void SetFullscreen(bool p_enabled)
        {
            Screen.fullScreen = p_enabled;
        }
    }
}
