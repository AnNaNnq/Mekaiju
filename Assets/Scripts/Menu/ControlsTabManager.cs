using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Mekaiju.UI
{
    public class ControlsTabManager : MonoBehaviour
    {
        // UI elements
        public PlayerInput playerInput;
        public Transform controlsContainer;
        public GameObject controlPrefab;

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
            foreach (InputAction t_action in playerInput.actions)
            {
                if (t_action.bindings.Count == 0) continue;

                // Create a button for each control setting
                GameObject t_obj = Instantiate(controlPrefab, controlsContainer);
                Button t_button = t_obj.GetComponent<Button>();
                Text t_buttonText = t_button.GetComponentInChildren<Text>();

                string t_actionName = t_action.name;
                t_buttonText.text = $"{t_actionName}: {t_action.GetBindingDisplayString()}";

                // Assign a function to handle rebinding
                t_button.onClick.AddListener(() => _StartRebind(t_action, t_buttonText));

                _buttons[t_actionName] = t_button;
            }
        }

        private void _StartRebind(InputAction p_action, Text p_buttonText)
        {
            // Disable action temporarily
            p_action.Disable();

            _rebindingOperation = p_action.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation =>
                {
                    // Update button text and save the new binding
                    p_action.Enable();
                    p_buttonText.text = $"{p_action.name}: {p_action.GetBindingDisplayString()}";
                    _SaveControl(p_action);
                    _rebindingOperation.Dispose();
                })
                .Start();
        }

        private void _SaveControl(InputAction p_action)
        {
            // Save new key binding
            string t_bindingJson = p_action.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString($"binding_{p_action.name}", t_bindingJson);
            PlayerPrefs.Save();
        }

        private void _LoadSavedBindings()
        {
            foreach (InputAction p_action in playerInput.actions)
            {
                // Load saved key bindings
                string t_bindingJson = PlayerPrefs.GetString($"binding_{p_action.name}", "");
                if (!string.IsNullOrEmpty(t_bindingJson))
                {
                    p_action.LoadBindingOverridesFromJson(t_bindingJson);
                }
            }
        }
    }
}