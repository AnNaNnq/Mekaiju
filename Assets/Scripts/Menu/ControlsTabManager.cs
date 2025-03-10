using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

namespace Mekaiju.Menu
{
    public class ControlsTabManager : MonoBehaviour
    {
        [Header("UI TMP Text References")]
        // UI elements to display key bindings for keyboard and gamepad
        public TMP_Text upText, upGamePadText;
        public TMP_Text downText, downGamePadText;
        public TMP_Text leftText, leftGamePadText;
        public TMP_Text rightText, rightGamePadText;
        public TMP_Text jumpText, jumpGamePadText;
        public TMP_Text cameraText, cameraGamePadText;
        public TMP_Text lockChangeText, lockChangeGamePadText;
        public TMP_Text lockOnOffText, lockOnOffGamePadText;
        public TMP_Text shieldText, shieldGamePadText;
        public TMP_Text leftArmActionText, leftArmActionGamePadText;
        public TMP_Text rightArmActionText, rightArmActionGamePadText;
        public TMP_Text dashText, dashGamePadText;
        public TMP_Text torseText, torseGamePadText;
        public TMP_Text headText, headGamePadText;
        public TMP_Text healText, healGamePadText;
        public TMP_Text pauseText, pauseGamePadText;

        [Header("Reference to the InputActionAsset")]
        // Reference to the input actions asset that contains all input configurations
        public InputActionAsset inputActions;

        // Start is called before the first frame update
        private void Start()
        {
            _DisplayKeybinds(); // Initialize key binding display on start
        }

        // Method to display the key bindings for the player controls
        private void _DisplayKeybinds()
        {
            // Retrieve the "Player" action map from the InputActionAsset
            InputActionMap t_playerMap = inputActions.FindActionMap("Player", throwIfNotFound: false);
            if (t_playerMap == null)
            {
                return;
            }

            // Update key binding text for various actions by calling _UpdateActionText for each control
            _UpdateActionText(t_playerMap, "Move", upText, upGamePadText, 1);
            _UpdateActionText(t_playerMap, "Move", downText, downGamePadText, 2);
            _UpdateActionText(t_playerMap, "Move", leftText, leftGamePadText, 3);
            _UpdateActionText(t_playerMap, "Move", rightText, rightGamePadText, 4);
            _UpdateActionText(t_playerMap, "Jump", jumpText, jumpGamePadText);
            _UpdateActionText(t_playerMap, "Look", cameraText, cameraGamePadText);
            _UpdateActionText(t_playerMap, "Lock", lockChangeText, lockChangeGamePadText);
            _UpdateActionText(t_playerMap, "LockSwitch", lockOnOffText, lockOnOffGamePadText);
            _UpdateActionText(t_playerMap, "Shield", shieldText, shieldGamePadText);
            _UpdateActionText(t_playerMap, "LeftArm", leftArmActionText, leftArmActionGamePadText);
            _UpdateActionText(t_playerMap, "RightArm", rightArmActionText, rightArmActionGamePadText);
            _UpdateActionText(t_playerMap, "Dash", dashText, dashGamePadText);
            _UpdateActionText(t_playerMap, "Torse", torseText, torseGamePadText);
            _UpdateActionText(t_playerMap, "Head", headText, headGamePadText);
            _UpdateActionText(t_playerMap, "Heal", healText, healGamePadText);
            _UpdateActionText(t_playerMap, "Pause", pauseText, pauseGamePadText);
        }

        // Method to update the key binding text for a specific action
        // Takes in action map, action name, UI text references for keyboard and gamepad, and an optional index
        private void _UpdateActionText(InputActionMap p_map, string p_actionName, TMP_Text p_keyboardText, TMP_Text p_gamepadText, int p_index = 0)
        {
            // Find the action in the action map
            InputAction t_action = p_map.FindAction(p_actionName, throwIfNotFound: false);
            if (t_action == null) return;

            // Temporary variables to store the current key bindings for both keyboard and gamepad
            string t_keyboardBinding = "Not Assigned";
            string t_gamepadBinding = "Not Assigned";

            // Iterate through the bindings of the action to find the appropriate key/mouse or gamepad input
            foreach (var t_binding in t_action.bindings)
            {
                if (t_binding.isComposite) continue; // Skip composite bindings (e.g., multi-button combinations)

                if (t_binding.path.Contains("<Keyboard>") || t_binding.path.Contains("<Mouse>"))
                {
                    t_keyboardBinding = t_binding.ToDisplayString(); // Assign the keyboard/mouse binding
                }
                else if (t_binding.path.Contains("<Gamepad>"))
                {
                    t_gamepadBinding = t_binding.ToDisplayString(); // Assign the gamepad binding
                }
                else if (t_binding.path.Contains("Pointer")) // Additional check for pointer input (mouse or touch)
                {
                    t_keyboardBinding = t_binding.ToDisplayString();
                }
            }

            // Update the UI text elements with the detected bindings for both keyboard and gamepad
            if (p_keyboardText) p_keyboardText.text = t_keyboardBinding;
            if (p_gamepadText) p_gamepadText.text = t_gamepadBinding;
        }
    }
}
