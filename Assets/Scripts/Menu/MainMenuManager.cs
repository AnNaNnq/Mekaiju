using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mekaiju.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        public string NextSceneName = "PauseManager"; // Name of the scene to load
        public GameObject MainMenu; // Reference to the main menu
        public GameObject SettingsMenu; // Reference to the settings menu
        public GameObject CreditsMenu; // Reference to the credits menu

        // Load the game scene
        public void PlayGame()
        {
            SceneManager.LoadScene(NextSceneName);
        }

        // Open the settings menu and hide the main menu
        public void OpenSettings()
        {
            MainMenu.SetActive(false);
            SettingsMenu.SetActive(true);
        }

        // Open the credits menu and hide the main menu
        public void OpenCredits()
        {
            MainMenu.SetActive(false);
            CreditsMenu.SetActive(true);
        }

        // Return to the main menu from any submenu
        public void BackToMainMenu()
        {
            SettingsMenu.SetActive(false);
            CreditsMenu.SetActive(false);
            MainMenu.SetActive(true);
        }

        // Exit the game
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
