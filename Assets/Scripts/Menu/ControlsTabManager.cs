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
        // Reference to the input actions asset
        public InputActionAsset inputActions;

        private void Start()
        {
            _DisplayKeybinds(); // Initialize key binding display on start
        }

        private void _DisplayKeybinds()
        {
            // Retrieve the "Player" action map from the InputActionAsset
            InputActionMap playerMap = inputActions.FindActionMap("Player", throwIfNotFound: false);
            if (playerMap == null)
            {
                Debug.LogError("Could not find the 'Player' Action Map in the InputActionAsset.");
                return;
            }

            // Update key binding text for various actions
            _UpdateActionText(playerMap, "Move", upText, upGamePadText, 1);
            _UpdateActionText(playerMap, "Move", downText, downGamePadText, 2);
            _UpdateActionText(playerMap, "Move", leftText, leftGamePadText, 3);
            _UpdateActionText(playerMap, "Move", rightText, rightGamePadText, 4);
            _UpdateActionText(playerMap, "Jump", jumpText, jumpGamePadText);
            _UpdateActionText(playerMap, "Look", cameraText, cameraGamePadText);
            _UpdateActionText(playerMap, "Lock", lockChangeText, lockChangeGamePadText);
            _UpdateActionText(playerMap, "LockSwitch", lockOnOffText, lockOnOffGamePadText);
            _UpdateActionText(playerMap, "Shield", shieldText, shieldGamePadText);
            _UpdateActionText(playerMap, "LeftArm", leftArmActionText, leftArmActionGamePadText);
            _UpdateActionText(playerMap, "RightArm", rightArmActionText, rightArmActionGamePadText);
            _UpdateActionText(playerMap, "Dash", dashText, dashGamePadText);
            _UpdateActionText(playerMap, "Torse", torseText, torseGamePadText);
            _UpdateActionText(playerMap, "Head", headText, headGamePadText);
            _UpdateActionText(playerMap, "Heal", healText, healGamePadText);
            _UpdateActionText(playerMap, "Pause", pauseText, pauseGamePadText);
        }

        private void _UpdateActionText(InputActionMap map, string actionName, TMP_Text keyboardText, TMP_Text gamepadText, int index = 0)
        {
            // Find the action in the map
            InputAction action = map.FindAction(actionName, throwIfNotFound: false);
            if (action == null) return;

            string keyboardBinding = "Not Assigned";
            string gamepadBinding = "Not Assigned";

            // Iterate through action bindings to determine their inputs
            foreach (var binding in action.bindings)
            {
                if (binding.isComposite) continue; // Skip composite bindings

                if (binding.path.Contains("<Keyboard>") || binding.path.Contains("<Mouse>"))
                {
                    keyboardBinding = binding.ToDisplayString(); // Assign keyboard/mouse binding
                }
                else if (binding.path.Contains("<Gamepad>"))
                {
                    gamepadBinding = binding.ToDisplayString(); // Assign gamepad binding
                }
                else if (binding.path.Contains("Pointer")) // Additional check for pointer input
                {
                    keyboardBinding = binding.ToDisplayString();
                }
            }

            // Update UI text with detected bindings
            if (keyboardText) keyboardText.text = keyboardBinding;
            if (gamepadText) gamepadText.text = gamepadBinding;
        }
    }
}
