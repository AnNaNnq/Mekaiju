using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mekaiju.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        public string nextSceneName = "Prototype"; // Name of the scene to load
        public GameObject mainMenu; // Reference to the main menu
        public GameObject settingsMenu; // Reference to the settings menu
        public GameObject creditsMenu; // Reference to the credits menu

        // Load the game scene
        public void PlayGame()
        {
            SceneManager.LoadScene(nextSceneName);
        }

        // Open the settings menu and hide the main menu
        public void OpenSettings()
        {
            mainMenu.SetActive(false);
            settingsMenu.SetActive(true);
        }

        // Open the credits menu and hide the main menu
        public void OpenCredits()
        {
            mainMenu.SetActive(false);
            creditsMenu.SetActive(true);
        }

        // Return to the main menu from any submenu
        public void BackToMainMenu()
        {
            settingsMenu.SetActive(false);
            creditsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }

        // Exit the game
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
