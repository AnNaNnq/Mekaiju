using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Mekaiju.Menu
{
    [System.Serializable]
    public class CreditSection
    {
        public string categoryName; // Name of the category (e.g., "Developers")
        public List<string> names = new List<string>(); // List of names in this category
        public List<Sprite> images = new List<Sprite>(); // List of images to display
    }

    public class CreditsManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private RectTransform _creditsTransform; // Container for credits
        [SerializeField] private GameObject _sectionPrefab; // Prefab containing the pre-configured VerticalLayoutGroup
        [SerializeField] private GameObject _imagePrefab; // Prefab for images
        [SerializeField] private GameObject _textPrefab; // Prefab for text
        [SerializeField] private float _scrollSpeed = 30f; // Scrolling speed

        [Header("Credits Data")]
        [SerializeField] private List<CreditSection> _creditSections = new List<CreditSection>();

        private void OnEnable()
        {
            _ResetCreditsPosition();
            _GenerateCreditsContent();
        }

        private void Update()
        {
            _creditsTransform.anchoredPosition += new Vector2(0, _scrollSpeed * Time.deltaTime);
        }

        private void _GenerateCreditsContent()
        {
            // Clear previous content to avoid duplication
            foreach (Transform child in _creditsTransform)
            {
                Destroy(child.gameObject);
            }

            foreach (var section in _creditSections)
            {
                // Use the section prefab that is already configured in the inspector
                GameObject sectionContainer = Instantiate(_sectionPrefab, _creditsTransform);

                // Add the image (if available)
                if (section.images.Count > 0)
                {
                    GameObject imgObj = Instantiate(_imagePrefab, sectionContainer.transform);
                    Image imgComponent = imgObj.GetComponent<Image>();
                    imgComponent.sprite = section.images[0];
                }

                // Add the text for the category and names
                GameObject textObj = Instantiate(_textPrefab, sectionContainer.transform);
                TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
                textComponent.text = $"\n<b>{section.categoryName}</b>\n"; // Add a space before

                foreach (var name in section.names)
                {
                    textComponent.text += $"{name}\n";
                }
            }
        }

        private void _ResetCreditsPosition()
        {
            _creditsTransform.anchoredPosition = Vector2.zero;
        }
    }
}
