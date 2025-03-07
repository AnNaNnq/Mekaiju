using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Mekaiju.UI
{
    [System.Serializable]
    public class CreditSection
    {
        public string categoryName; // Name of the category (e.g. "Developers")
        public List<string> names = new List<string>(); // List of names in this category
        public List<Sprite> images = new List<Sprite>(); // List of images to display
    }

    public class CreditsManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _creditsText; // Single TextMeshProUGUI to display categories and names
        [SerializeField] private RectTransform _creditsTransform; // RectTransform to scroll the content
        [SerializeField] private GameObject _imagePrefab; // Image prefab to instantiate images
        [SerializeField] private float _scrollSpeed = 30f; // Speed of the scrolling effect

        [Header("Credits Data")]
        [SerializeField] private List<CreditSection> _creditSections = new List<CreditSection>(); // List of all credit sections

        // Called when the script is enabled to set up the credits
        private void OnEnable()
        {
            _ResetCreditsPosition(); // Reset the scroll position to the top
            _GenerateCreditsContent(); // Generate content with images and text
        }

        // Called every frame to scroll the credits upwards
        private void Update()
        {
            _creditsTransform.Translate(Vector3.up * (_scrollSpeed * Time.deltaTime)); // Move content upwards
        }

        // Method to generate the content (images + text) for the credits
        private void _GenerateCreditsContent()
        {
            // Clear existing content
            _creditsText.text = ""; // Clear any existing text

            // Loop through each credit section
            foreach (var section in _creditSections)
            {
                // Add the first image (if any)
                if (section.images.Count > 0)
                {
                    GameObject imgObj = Instantiate(_imagePrefab, _creditsTransform); // Instantiate a new image
                    Image imgComponent = imgObj.GetComponent<Image>(); // Get the Image component
                    imgComponent.sprite = section.images[0]; // Set the image to the first in the list
                    imgComponent.rectTransform.sizeDelta = new Vector2(200, 200); // Set the image size
                    imgComponent.rectTransform.anchoredPosition = Vector2.zero; // Position image
                }

                // Add category title (e.g., "Developers")
                _creditsText.text += $"<b>{section.categoryName}</b>\n"; // Bold category name

                // Add all the names in this section
                foreach (var name in section.names)
                {
                    _creditsText.text += $"{name}\n"; // Add each name
                }

                _creditsText.text += "\n"; // Add a space between sections

                // Add remaining images for the section (if any)
                for (int i = 1; i < section.images.Count; i++) // Start from the second image
                {
                    GameObject imgObj = Instantiate(_imagePrefab, _creditsTransform); // Instantiate a new image
                    Image imgComponent = imgObj.GetComponent<Image>(); // Get the Image component
                    imgComponent.sprite = section.images[i]; // Set the image sprite
                    imgComponent.rectTransform.sizeDelta = new Vector2(200, 200); // Set image size
                    imgComponent.rectTransform.anchoredPosition = Vector2.zero; // Position image
                }
            }
        }

        // Method to reset the credits' position to the top of the container
        private void _ResetCreditsPosition()
        {
            _creditsTransform.anchoredPosition = new Vector2(_creditsTransform.anchoredPosition.x, 0); // Reset position
        }
    }
}
