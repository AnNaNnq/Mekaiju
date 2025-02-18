using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Mekaiju.UI
{
    [System.Serializable]
    public class CreditSection
    {
        public string CategoryName;
        public List<string> Names = new List<string>();
    }

    public class CreditsManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI categoriesText;
        [SerializeField] private TextMeshProUGUI namesText;
        [SerializeField] private RectTransform creditsTransform;
        [SerializeField] private float scrollSpeed = 30f;

        [Header("Credits Data")]
        [SerializeField] private List<CreditSection> creditSections = new List<CreditSection>();

        private void OnEnable()
        {
            _ResetCreditsPosition();
            GenerateCreditsText();
        }

        private void Update()
        {
            creditsTransform.Translate(Vector3.up * (scrollSpeed * Time.deltaTime));
        }

        private void GenerateCreditsText()
        {
            categoriesText.text = "";
            namesText.text = "";

            foreach (var section in creditSections)
            {
                categoriesText.text += $"<b>{section.CategoryName}</b>\n";

                foreach (var name in section.Names)
                {
                    namesText.text += $"{name}\n";
                }

                categoriesText.text += "\n"; // Space between sections
                namesText.text += "\n";
            }
        }

        private void _ResetCreditsPosition()
        {
            creditsTransform.anchoredPosition = new Vector2(creditsTransform.anchoredPosition.x, 0);
        }
    }
}
