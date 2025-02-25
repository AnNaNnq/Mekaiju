using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Mekaiju.Menu
{
    [System.Serializable]
    public class CreditSection
    {
        public string categoryName; // The name of the credit category
        public List<string> names = new List<string>(); // A list of names for this credit category

    }

    public class CreditsManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _categoriesText; // Text UI element to display credit categories
        [SerializeField] private TextMeshProUGUI _namesText; // Text UI element to display credit names
        [SerializeField] private RectTransform _creditsTransform; // RectTransform for moving the credits vertically
        [SerializeField] private float _scrollSpeed = 30f; // Speed at which the credits scroll upward

        [Header("Credits Data")]
        [SerializeField] private List<CreditSection> _creditSections = new List<CreditSection>(); // List of credit sections to display

        // Called when the object becomes enabled and active
        private void OnEnable()
        {
            _ResetCreditsPosition(); // Reset the position of the credits
            _GenerateCreditsText(); // Generate the text content for the credits UI elements
        }

        private void Update()
        {
            // Scroll the credits upward over time based on the scroll speed and frame time
            _creditsTransform.Translate(Vector3.up * (_scrollSpeed * Time.deltaTime));
        }

        // Generates the text for the credits by combining categories and names
        private void _GenerateCreditsText()
        {
            // Clear existing text from the UI elements
            _categoriesText.text = "";
            _namesText.text = "";

            // Loop through each credit section
            foreach (var t_section in _creditSections)
            {
                // Append the category name in bold, followed by a newline
                _categoriesText.text += $"<b>{t_section.categoryName}</b>\n";

                // Append each name under the current category
                foreach (var name in t_section.names)
                {
                    _namesText.text += $"{name}\n";
                }
                // Add an extra newline for spacing between sections
                _categoriesText.text += "\n"; // Space between sections
                _namesText.text += "\n";
            }
        }
        // Resets the position of the credits to the starting point
        private void _ResetCreditsPosition()
        {
            // Reset the vertical position while keeping the current horizontal position
            _creditsTransform.anchoredPosition = new Vector2(_creditsTransform.anchoredPosition.x, 0);
        }
    }
}
