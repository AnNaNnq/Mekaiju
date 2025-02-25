using UnityEngine;
using TMPro; 
using UnityEngine.InputSystem; 


namespace Mekaiju.Menu
{
public class ControlsTabManager : MonoBehaviour
{
    // These variables hold references to your TextMeshPro UI elements.
    [Header("UI TMP Text References")]
    public TMP_Text upText;
    public TMP_Text downText;
    public TMP_Text leftText;
    public TMP_Text rightText;
    public TMP_Text jumpText;
    public TMP_Text cameraText;
    public TMP_Text lockChangeText;
    public TMP_Text lockOnOffText;
    public TMP_Text shieldText;
    public TMP_Text leftArmActionText;
    public TMP_Text rightArmActionText;
    public TMP_Text dashText;
    public TMP_Text chestText;
    public TMP_Text headText;
    public TMP_Text healText;
    public TMP_Text pauseText;

        // This is the InputActionAsset that contains your input mappings (the .inputactions file).
        [Header("Reference to the InputActionAsset")]
    public InputActionAsset inputActions;

    private void Start()
    {
        // Update the UI with the current keybinds from the InputActionAsset.
        _DisplayKeybinds();
    }

    // Retrieves the keybindings from the InputActionAsset and updates the TMP texts.
    private void _DisplayKeybinds()
    {
        // Attempt to find the Action Map named "Player" in the InputActionAsset.
        InputActionMap playerMap = inputActions.FindActionMap("Player", throwIfNotFound: false);
        // If the "Player" Action Map isn't found, log an error and exit the method.
        if (playerMap == null)
        {
            Debug.LogError("Could not find the 'Player' Action Map in the InputActionAsset.");
            return;
        }

        // For each action, we try to retrieve its first binding and then update
        // the corresponding TMP text with a human-readable string.
        // If an action or its binding isn't found, nothing happens.

        // Update the "Up" action text.
        if (upText)
        {
            InputAction action = playerMap.FindAction("Up", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                upText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "Down" action text.
        if (downText)
        {
            InputAction action = playerMap.FindAction("Down", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                downText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "Left" action text.
        if (leftText)
        {
            InputAction action = playerMap.FindAction("Left", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                leftText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "Right" action text.
        if (rightText)
        {
            InputAction action = playerMap.FindAction("Right", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                rightText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "Jump" action text.
        if (jumpText)
        {
            InputAction action = playerMap.FindAction("Jump", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                jumpText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "Camera" action text.
        if (cameraText)
        {
            InputAction action = playerMap.FindAction("Camera", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                cameraText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "LockChange" action text.
        if (lockChangeText)
        {
            InputAction action = playerMap.FindAction("Lock", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                lockChangeText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "LockOn/Off" action text.
        if (lockOnOffText)
        {
            InputAction action = playerMap.FindAction("LockSwitch", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                lockOnOffText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "Shield" action text.
        if (shieldText)
        {
            InputAction action = playerMap.FindAction("Shield", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                shieldText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "LeftArmAction" action text.
        if (leftArmActionText)
        {
            InputAction action = playerMap.FindAction("LeftArmAction", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                leftArmActionText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "RightArmAction" action text.
        if (rightArmActionText)
        {
            InputAction action = playerMap.FindAction("RightArmAction", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                rightArmActionText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "Dash" action text.
        if (dashText)
        {
            InputAction action = playerMap.FindAction("Dash", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                dashText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "Chest" action text.
        if (chestText)
        {
            InputAction action = playerMap.FindAction("Chest", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                chestText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "Head" action text.
        if (headText)
        {
            InputAction action = playerMap.FindAction("Head", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                headText.text = action.bindings[0].ToDisplayString();
        }
        // Update the "Heal" action text.
        if (healText)
        {
            InputAction action = playerMap.FindAction("Heal", throwIfNotFound: false);
            if (action != null && action.bindings.Count > 0)
                healText.text = action.bindings[0].ToDisplayString();
        }
            // Update the "Pause" action text.
            if (healText)
            {
                InputAction action = playerMap.FindAction("Pause", throwIfNotFound: false);
                if (action != null && action.bindings.Count > 0)
                    pauseText.text = action.bindings[0].ToDisplayString();
            }
        }
}

}

