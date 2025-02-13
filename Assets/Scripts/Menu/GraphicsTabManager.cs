using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Mekaiju.Graphics
{
    public class GraphicsTabManager : MonoBehaviour
    {
        // UI elements
        public TMP_Dropdown resolutionDropdown;  // Dropdown for screen resolution selection
        public TMP_Dropdown qualityDropdown;     // Dropdown for graphics quality selection
        public Toggle reflectionsToggle;     // Toggle for real-time reflections
        public Toggle vSyncToggle;           // Toggle for V-Sync
        public Toggle fullscreenToggle;      // Toggle for fullscreen mode

        private Resolution[] _resolutions;   // Available screen resolutions

        void Start()
        {
            // Load available screen resolutions
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;
                options.Add(option);

                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            // Load quality settings
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.RefreshShownValue();

            // Load toggle settings
            reflectionsToggle.isOn = PlayerPrefs.GetInt("Reflections", 1) == 1;
            vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
            fullscreenToggle.isOn = Screen.fullScreen;

            // Add event listeners
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            qualityDropdown.onValueChanged.AddListener(SetQuality);
            reflectionsToggle.onValueChanged.AddListener(SetReflections);
            vSyncToggle.onValueChanged.AddListener(SetVSync);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        private void SetResolution(int index)
        {
            Screen.SetResolution(_resolutions[index].width, _resolutions[index].height, Screen.fullScreen);
        }

        private void SetQuality(int index)
        {
            QualitySettings.SetQualityLevel(index);
        }

        private void SetReflections(bool enabled)
        {
            PlayerPrefs.SetInt("Reflections", enabled ? 1 : 0);
        }

        private void SetVSync(bool enabled)
        {
            QualitySettings.vSyncCount = enabled ? 1 : 0;
        }

        private void SetFullscreen(bool enabled)
        {
            Screen.fullScreen = enabled;
        }
    }
}
