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
        public void ShowPanel(string p_PanelName)
        {
            // Hide all panels
            GraphicsPanel.SetActive(false);
            AudioPanel.SetActive(false);
            ControlsPanel.SetActive(false);

            // Show the selected panel based on its name
            switch (p_PanelName)
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
            UpdateButtonAppearance(p_PanelName);
        }

        // Update the appearance of the buttons
        private void UpdateButtonAppearance(string p_ActivePanel)
        {
            // Reset all buttons to inactive color
            SetInactiveButton(GraphicsButton);
            SetInactiveButton(AudioButton);
            SetInactiveButton(ControlsButton);

            // Set the active button based on the active panel
            if (p_ActivePanel == "Graphics")
            {
                SetActiveButton(GraphicsButton);
            }
            else if (p_ActivePanel == "Audio")
            {
                SetActiveButton(AudioButton);
            }
            else if (p_ActivePanel == "Controls")
            {
                SetActiveButton(ControlsButton);
            }
        }

        // Set a button to active color
        private void SetActiveButton(Button t_Button)
        {
            ColorBlock buttonColors = t_Button.colors;
            buttonColors.normalColor = ActiveButtonColor;
            t_Button.colors = buttonColors;
        }

        // Set a button to inactive color
        private void SetInactiveButton(Button t_Button)
        {
            ColorBlock buttonColors = t_Button.colors;
            buttonColors.normalColor = InactiveButtonColor;
            t_Button.colors = buttonColors;
        }
    }
}
