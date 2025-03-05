using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

namespace Mekaiju.Menu
{
    public class GraphicsTabManager : MonoBehaviour
    {
        // UI elements
        public TMP_Dropdown resolutionDropdown;  // Dropdown for screen resolution selection
        public TMP_Dropdown qualityDropdown;     // Dropdown for graphics quality selection
        public Toggle reflectionsToggle;         // Toggle for real-time reflections
        public Toggle vSyncToggle;               // Toggle for V-Sync
        public Toggle fullscreenToggle;          // Toggle for fullscreen mode

        private Resolution[] _resolutions;       // Available screen resolutions
        private List<Resolution> _filteredResolutions; // Filtered unique resolutions

        void Start()
        {
            // Load available screen resolutions
            _resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            HashSet<string> resolutionSet = new HashSet<string>();
            _filteredResolutions = new List<Resolution>();

            int t_currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string t_option = _resolutions[i].width + " x " + _resolutions[i].height;

                if (resolutionSet.Add(t_option)) // Add resolution only if it's not already in the set
                {
                    _filteredResolutions.Add(_resolutions[i]);
                }

                //Check if this resolution matches the current screen resolution
                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    t_currentResolutionIndex = _filteredResolutions.Count - 1;
                }
            }

            // Convert to string list and add to the dropdown options
            List<string> t_options = _filteredResolutions.Select(r => r.width + " x " + r.height).ToList();
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

        private void _SetResolution(int p_index)
        {
            Screen.SetResolution(_filteredResolutions[p_index].width, _filteredResolutions[p_index].height, Screen.fullScreen);
        }

        private void _SetQuality(int p_index)
        {
            QualitySettings.SetQualityLevel(p_index);
        }

        private void _SetReflections(bool p_enabled)
        {
            PlayerPrefs.SetInt("Reflections", p_enabled ? 1 : 0);
        }

        private void _SetVSync(bool p_enabled)
        {
            QualitySettings.vSyncCount = p_enabled ? 1 : 0;
        }

        private void _SetFullscreen(bool p_enabled)
        {
            Screen.fullScreen = p_enabled;
        }
    }
}
