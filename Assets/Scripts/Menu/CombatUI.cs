using Mekaiju;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mekaiju.UI
{
    public class CombatMenu : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _pauseMenu; 
        [SerializeField] 
        private GameObject _settingsMenu;

        [SerializeField] 
        private GameObject _looseMenu;
        [SerializeField] 
        private GameObject _winMenu;

        [SerializeField] 
        private Button _resumeButton;

        private CombatManager _manager;
        private bool _isPaused;

        private void Awake()
        {
            _pauseMenu.SetActive(false); 
            _settingsMenu.SetActive(false);
            _winMenu.SetActive(false);
            _looseMenu.SetActive(false);

            _manager  = GameObject.Find("CombatManager").GetComponent<CombatManager>();
            _isPaused = false;   
        }

        private void Start()
        {
            _manager.onStateChange.AddListener(_OnCombatStateChange);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_settingsMenu.activeSelf)
                {
                    CloseSettings();
                }
                else
                {
                    TogglePause();
                }
            }        
        }

        private void _OnCombatStateChange(CombatState p_state)
        {
            if (p_state == CombatState.Ended)
            {
                _UnlockCursor();
                if (_manager.result == CombatResult.Win)
                {
                    _winMenu.SetActive(true);
                }
                else
                {
                    _looseMenu.SetActive(true);
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

        public void ReturnMainMenu()
        {
            SceneManager.LoadScene("MainScene");
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
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