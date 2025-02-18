using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Mekaiju.Controls
{
    public class ControlsTabManager : MonoBehaviour
    {
        // UI elements
        public PlayerInput playerInput;
        public Transform _controlsContainer;
        public GameObject _controlPrefab;

        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
        private Dictionary<string, Button> _buttons = new Dictionary<string, Button>();

        void Start()
        {
            // Load saved key bindings and create UI
            _LoadSavedBindings();
            _LoadControls();
        }

        private void _LoadControls()
        {
            foreach (InputAction action in playerInput.actions)
            {
                if (action.bindings.Count == 0) continue;

                // Create a button for each control setting
                GameObject obj = Instantiate(_controlPrefab, _controlsContainer);
                Button button = obj.GetComponent<Button>();
                Text buttonText = button.GetComponentInChildren<Text>();

                string actionName = action.name;
                buttonText.text = $"{actionName}: {action.GetBindingDisplayString()}";

                // Assign a function to handle rebinding
                button.onClick.AddListener(() => _StartRebind(action, buttonText));

                _buttons[actionName] = button;
            }
        }

        private void _StartRebind(InputAction action, Text buttonText)
        {
            // Disable action temporarily
            action.Disable();

            _rebindingOperation = action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation =>
                {
                    // Update button text and save the new binding
                    action.Enable();
                    buttonText.text = $"{action.name}: {action.GetBindingDisplayString()}";
                    _SaveControl(action);
                    _rebindingOperation.Dispose();
                })
                .Start();
        }

        private void _SaveControl(InputAction action)
        {
            // Save new key binding
            string bindingJson = action.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString($"binding_{action.name}", bindingJson);
            PlayerPrefs.Save();
        }

        private void _LoadSavedBindings()
        {
            foreach (InputAction action in playerInput.actions)
            {
                // Load saved key bindings
                string bindingJson = PlayerPrefs.GetString($"binding_{action.name}", "");
                if (!string.IsNullOrEmpty(bindingJson))
                {
                    action.LoadBindingOverridesFromJson(bindingJson);
                }
            }
        }
    }
}
