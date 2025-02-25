using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Mekaiju.Menu
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
            resolutionDropdown.ClearOptions(); // Clear any pre-existing options in the resolution dropdown
            List<string> t_options = new List<string>(); // List to hold the resolution options as strings

            int t_currentResolutionIndex = 0; // Variable to store the index of the current screen resolution
            for (int i = 0; i < _resolutions.Length; i++)
            {
                // Create a string representation of the resolution
                string t_option = _resolutions[i].width + " x " + _resolutions[i].height;
                t_options.Add(t_option);

                // Check if this resolution matches the current screen resolution
                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    t_currentResolutionIndex = i;
                }
            }

            // Add the resolution options to the dropdown and set the default selected value
            resolutionDropdown.AddOptions(t_options);
            resolutionDropdown.value = t_currentResolutionIndex;
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
            resolutionDropdown.onValueChanged.AddListener(p_index => _SetResolution(p_index));
            qualityDropdown.onValueChanged.AddListener(p_index => _SetQuality(p_index));
            reflectionsToggle.onValueChanged.AddListener(p_enabled => _SetReflections(p_enabled));
            vSyncToggle.onValueChanged.AddListener(p_enabled => _SetVSync(p_enabled));
            fullscreenToggle.onValueChanged.AddListener(p_enabled => _SetFullscreen(p_enabled));
        }
        // Method to update the screen resolution based on the user's selection
        private void _SetResolution(int p_index)
        {
            Screen.SetResolution(_resolutions[p_index].width, _resolutions[p_index].height, Screen.fullScreen);
        }
        // Method to update the graphics quality level based on the user's selection
        private void _SetQuality(int p_index)
        {
            QualitySettings.SetQualityLevel(p_index);
        }
        // Method to enable or disable reflections and save the preference
        private void _SetReflections(bool p_enabled)
        {
            PlayerPrefs.SetInt("Reflections", p_enabled ? 1 : 0);
        }
        // Method to enable or disable V-Sync based on the user's toggle
        private void _SetVSync(bool p_enabled)
        {
            QualitySettings.vSyncCount = p_enabled ? 1 : 0;
        }
        // Method to enable or disable fullscreen mode
        private void _SetFullscreen(bool p_enabled)
        {
            Screen.fullScreen = p_enabled;
        }
    }
}
