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
        [SerializeField] private AudioSource[] _uiAudioSources; // UI audio sources (menu sounds)

        private void Start()
        {
            _pauseMenu.SetActive(false); // Hide pause menu at start
            _settingsMenu.SetActive(false); // Hide settings menu at start
            _LockCursor(); // Lock the cursor at the start

            // Ensure UI sounds are not paused when the game is paused
            if (_uiAudioSources != null)
            {
                foreach (AudioSource audioSource in _uiAudioSources)
                {
                    if (audioSource != null)
                    {
                        audioSource.ignoreListenerPause = true;
                    }
                }
            }

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // Press "Esc" to pause/unpause
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

        // Toggles pause state
        public void TogglePause()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0f : 1f; // Pause or resume game by changing timescale
            _pauseMenu.SetActive(_isPaused); // Show or hide the pause menu
            AudioListener.pause = _isPaused; // Pause the sounds

            if (_isPaused)
            {
                _resumeButton.Select(); // Auto-select the resume button when paused
                _UnlockCursor(); // Unlock the cursor when paused
            }
            else
            {
                _LockCursor(); // Lock the cursor during gameplay
            }
        }

        // Opens the settings menu
        public void OpenSettings()
        {
            _pauseMenu.SetActive(false); // Hide pause menu
            _settingsMenu.SetActive(true); // Show settings menu
        }

        // Closes the settings menu and returns to the pause menu
        public void CloseSettings()
        {
            _settingsMenu.SetActive(false); // Hide settings menu
            _pauseMenu.SetActive(true); // Show pause menu again
        }

        // Locks the cursor to the center of the screen and makes it invisible
        private void _LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor at the center of the screen
            Cursor.visible = false; // Make the cursor invisible during gameplay
        }

        // Unlocks the cursor and makes it visible
        private void _UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Make the cursor visible for menu interaction
        }
    }
}
