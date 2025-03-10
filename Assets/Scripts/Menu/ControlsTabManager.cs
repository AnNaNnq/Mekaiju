using UnityEngine;
using TMPro;

namespace Mekaiju.Menu
{
    public class ControlsTabManager : MonoBehaviour
    {
        [Header("UI TMP Text References")]
        // UI text elements to display key bindings for both keyboard and gamepad
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

        private void Start()
        {
            _DisplayKeybinds(); // Initialize key binding display on start
        }

        // Method to set key bindings manually for each action
        private void _DisplayKeybinds()
        {
            _SetKeybind(upText, "W");
            _SetKeybind(upGamePadText, "LS ");
            _SetKeybind(downText, "S");
            _SetKeybind(downGamePadText, "LS ");
            _SetKeybind(leftText, "A");
            _SetKeybind(leftGamePadText, "LS ");
            _SetKeybind(rightText, "D");
            _SetKeybind(rightGamePadText, "LS ");
            _SetKeybind(jumpText, "Space");
            _SetKeybind(jumpGamePadText, "A");
            _SetKeybind(cameraText, "Delta");
            _SetKeybind(cameraGamePadText, "RS");
            _SetKeybind(lockChangeText, "Scroll/Y");
            _SetKeybind(lockChangeGamePadText, "D-Pad");
            _SetKeybind(lockOnOffText, "MMB");
            _SetKeybind(lockOnOffGamePadText, "Right Stick Press");
            _SetKeybind(shieldText, "Left Control");
            _SetKeybind(shieldGamePadText, "X");
            _SetKeybind(leftArmActionText, "LMB");
            _SetKeybind(leftArmActionGamePadText, "LT");
            _SetKeybind(rightArmActionText, "RMB");
            _SetKeybind(rightArmActionGamePadText, "RT");
            _SetKeybind(dashText, "Left Shift");
            _SetKeybind(dashGamePadText, "B");
            _SetKeybind(torseText, "E");
            _SetKeybind(torseGamePadText, "LB");
            _SetKeybind(headText, "Q");
            _SetKeybind(headGamePadText, "Y");
            _SetKeybind(healText, "R");
            _SetKeybind(healGamePadText, "RB");
            _SetKeybind(pauseText, "Escape");
            _SetKeybind(pauseGamePadText, "Start");
        }

        // Method to set text for a given UI element
        private void _SetKeybind(TMP_Text p_textElement, string p_keybind)
        {
            if (p_textElement != null)
            {
                p_textElement.text = p_keybind;
            }
        }
    }
}