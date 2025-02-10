using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Mekaiju.Pause
{
    public class PauseManager : MonoBehaviour
    {
        private bool _isPaused = false; // Pause state
        [SerializeField] private GameObject _pauseMenu; // Pause panel
        [SerializeField] private GameObject _settingsMenu; // Settings panel
        [SerializeField] private Button _resumeButton; // Default selected button
        
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _pauseMenu.SetActive(false); // Hide pause menu at start
            _settingsMenu.SetActive(false); // Hide settings menu at start
        }


        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_settingsMenu.activeSelf)
                {
                    CloseSettings(); // Close settings first if open
                }
                else
                {
                    TogglePause();
                }
            }
        }

        public void TogglePause()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f;
            _pauseMenu.SetActive(_isPaused);

            if (_isPaused)
            {
                _resumeButton.Select(); // Auto-select resume button
            }
        }

        public void OpenSettings()
        {
            _pauseMenu.SetActive(false); // Hide pause menu
            _settingsMenu.SetActive(true); // Show settings menu
        }

        public void CloseSettings()
        {
            _settingsMenu.SetActive(false); // Hide settings menu
            _pauseMenu.SetActive(true); // Show pause menu again
        }
    }
}
