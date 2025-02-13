using UnityEngine;
using UnityEngine.UI;

namespace Mekaiju.UI
{
    public class SettingsManager : MonoBehaviour
    {
        // References to the settings panels (Graphics, Audio, Controls)
        public GameObject GraphicsPanel;
        public GameObject AudioPanel;
        public GameObject ControlsPanel;

        // References to the buttons
        public Button GraphicsButton;
        public Button AudioButton;
        public Button ControlsButton;

        // Colors for active/inactive buttons
        public Color ActiveButtonColor;
        public Color InactiveButtonColor;

        void Start()
        {
            // Add listeners to buttons
            GraphicsButton.onClick.AddListener(() => ShowPanel("Graphics"));
            AudioButton.onClick.AddListener(() => ShowPanel("Audio"));
            ControlsButton.onClick.AddListener(() => ShowPanel("Controls"));

            // Initially show the "Graphics" panel and set its button to active color
            SetActiveButton(GraphicsButton);
            ShowPanel("Graphics");
        }

        // Show the selected panel and update button appearance
        public void ShowPanel(string panelName)
        {
            // Hide all panels
            GraphicsPanel.SetActive(false);
            AudioPanel.SetActive(false);
            ControlsPanel.SetActive(false);

            // Show the selected panel based on its name
            switch (panelName)
            {
                case "Graphics":
                    GraphicsPanel.SetActive(true);
                    break;
                case "Audio":
                    AudioPanel.SetActive(true);
                    break;
                case "Controls":
                    ControlsPanel.SetActive(true);
                    break;
            }

            // Update the appearance of the buttons (active/inactive)
            UpdateButtonAppearance(panelName);
        }

        // Update the appearance of the buttons
        private void UpdateButtonAppearance(string activePanel)
        {
            // Reset all buttons to inactive color
            SetInactiveButton(GraphicsButton);
            SetInactiveButton(AudioButton);
            SetInactiveButton(ControlsButton);

            // Set the active button based on the active panel
            if (activePanel == "Graphics")
            {
                SetActiveButton(GraphicsButton);
            }
            else if (activePanel == "Audio")
            {
                SetActiveButton(AudioButton);
            }
            else if (activePanel == "Controls")
            {
                SetActiveButton(ControlsButton);
            }
        }

        // Set a button to active color
        private void SetActiveButton(Button button)
        {
            ColorBlock buttonColors = button.colors;
            buttonColors.normalColor = ActiveButtonColor;
            button.colors = buttonColors;
        }

        // Set a button to inactive color
        private void SetInactiveButton(Button button)
        {
            ColorBlock buttonColors = button.colors;
            buttonColors.normalColor = InactiveButtonColor;
            button.colors = buttonColors;
        }
    }
}
