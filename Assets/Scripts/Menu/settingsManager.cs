using UnityEngine;
using UnityEngine.UI;

namespace Mekaiju.Menu

{
    public class SettingsManager : MonoBehaviour
    {
        // References to the settings panels (Graphics, Audio, Controls)
        public GameObject graphicsPanel;
        public GameObject audioPanel;
        public GameObject controlsPanel;

        // References to the buttons
        public Button graphicsButton;
        public Button audioButton;
        public Button controlsButton;

        // Colors for active/inactive buttons
        public Color activeButtonColor;
        public Color inactiveButtonColor;

        void Start()
        {
            // Add listeners to buttons
            graphicsButton.onClick.AddListener(() => ShowPanel("Graphics"));
            audioButton.onClick.AddListener(() => ShowPanel("Audio"));
            controlsButton.onClick.AddListener(() => ShowPanel("Controls"));

            // Initially show the "Graphics" panel and set its button to active color
            _SetActiveButton(graphicsButton);
            ShowPanel("Graphics");
        }

        // Show the selected panel and update button appearance
        public void ShowPanel(string p_PanelName)
        {
            // Hide all panels
            graphicsPanel.SetActive(false);
            audioPanel.SetActive(false);
            controlsPanel.SetActive(false);

            // Show the selected panel based on its name
            switch (p_PanelName)
            {
                case "Graphics":
                    graphicsPanel.SetActive(true);
                    break;
                case "Audio":
                    audioPanel.SetActive(true);
                    break;
                case "Controls":
                    controlsPanel.SetActive(true);
                    break;
            }

            // Update the appearance of the buttons (active/inactive)
            _UpdateButtonAppearance(p_PanelName);
        }

        // Update the appearance of the buttons
        private void _UpdateButtonAppearance(string p_ActivePanel)
        {
            // Reset all buttons to inactive color
            _SetInactiveButton(graphicsButton);
            _SetInactiveButton(audioButton);
            _SetInactiveButton(controlsButton);

            // Set the active button based on the active panel
            if (p_ActivePanel == "Graphics")
            {
                _SetActiveButton(graphicsButton);
            }
            else if (p_ActivePanel == "Audio")
            {
                _SetActiveButton(audioButton);
            }
            else if (p_ActivePanel == "Controls")
            {
                _SetActiveButton(controlsButton);
            }
        }

        // Set a button to active color
        private void _SetActiveButton(Button t_Button)
        {
            ColorBlock buttonColors = t_Button.colors;
            buttonColors.normalColor = activeButtonColor;
            t_Button.colors = buttonColors;
        }

        // Set a button to inactive color
        private void _SetInactiveButton(Button t_Button)
        {
            ColorBlock buttonColors = t_Button.colors;
            buttonColors.normalColor = inactiveButtonColor;
            t_Button.colors = buttonColors;
        }
    }
}
