using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Mekaiju.Controls
{
    public class ControlsTabManager : MonoBehaviour
    {
        // UI elements
        public PlayerInput PlayerInput;
        public Transform ControlsContainer;
        public GameObject ControlPrefab;

        private InputActionRebindingExtensions.RebindingOperation _rebindingOperation;
        private Dictionary<string, Button> _buttons = new Dictionary<string, Button>();

        void Start()
        {
            // Load saved key bindings and create UI
            LoadSavedBindings();
            LoadControls();
        }

        private void LoadControls()
        {
            foreach (InputAction p_action in PlayerInput.actions)
            {
                if (p_action.bindings.Count == 0) continue;

                // Create a button for each control setting
                GameObject t_obj = Instantiate(ControlPrefab, ControlsContainer);
                Button t_button = t_obj.GetComponent<Button>();
                Text t_buttonText = t_button.GetComponentInChildren<Text>();

                string actionName = p_action.name;
                t_buttonText.text = $"{actionName}: {p_action.GetBindingDisplayString()}";

                // Assign a function to handle rebinding
                t_button.onClick.AddListener(() => StartRebind(p_action, t_buttonText));

                _buttons[actionName] = t_button;
            }
        }

        private void StartRebind(InputAction p_action, Text t_buttonText)
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
                    t_buttonText.text = $"{p_action.name}: {p_action.GetBindingDisplayString()}";
                    SaveControl(p_action);
                    _rebindingOperation.Dispose();
                })
                .Start();
        }

        private void SaveControl(InputAction p_action)
        {
            // Save new key binding
            string t_bindingJson = p_action.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString($"binding_{p_action.name}", t_bindingJson);
            PlayerPrefs.Save();
        }

        private void LoadSavedBindings()
        {
            foreach (InputAction p_action in PlayerInput.actions)
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