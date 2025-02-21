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
        [SerializeField] private TextMeshProUGUI CategoriesText;
        [SerializeField] private TextMeshProUGUI NamesText;
        [SerializeField] private RectTransform CreditsTransform;
        [SerializeField] private float ScrollSpeed = 30f;

        [Header("Credits Data")]
        [SerializeField] private List<CreditSection> CreditSections = new List<CreditSection>();

        private void OnEnable()
        {
            ResetCreditsPosition();
            GenerateCreditsText();
        }

        private void Update()
        {
            CreditsTransform.Translate(Vector3.up * (ScrollSpeed * Time.deltaTime));
        }

        private void GenerateCreditsText()
        {
            CategoriesText.text = "";
            NamesText.text = "";

            foreach (var section in CreditSections)
            {
                CategoriesText.text += $"<b>{section.CategoryName}</b>\n";

                foreach (var name in section.Names)
                {
                    NamesText.text += $"{name}\n";
                }

                CategoriesText.text += "\n"; // Space between sections
                NamesText.text += "\n";
            }
        }

        private void ResetCreditsPosition()
        {
            CreditsTransform.anchoredPosition = new Vector2(CreditsTransform.anchoredPosition.x, 0);
        }
    }
}
