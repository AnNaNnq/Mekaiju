using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Mekaiju.UI
{
    [System.Serializable]
    public class CreditSection
    {
        public string categoryName;
        public List<string> names = new List<string>();
    }

    public class CreditsManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _categoriesText;
        [SerializeField] private TextMeshProUGUI _namesText;
        [SerializeField] private RectTransform _creditsTransform;
        [SerializeField] private float _scrollSpeed = 30f;

        [Header("Credits Data")]
        [SerializeField] private List<CreditSection> _creditSections = new List<CreditSection>();

        private void OnEnable()
        {
            _ResetCreditsPosition();
            _GenerateCreditsText();
        }

        private void Update()
        {
            _creditsTransform.Translate(Vector3.up * (_scrollSpeed * Time.deltaTime));
        }

        private void _GenerateCreditsText()
        {
            _categoriesText.text = "";
            _namesText.text = "";

            foreach (var t_section in _creditSections)
            {
                _categoriesText.text += $"<b>{t_section.categoryName}</b>\n";

                foreach (var name in t_section.names)
                {
                    _namesText.text += $"{name}\n";
                }

                _categoriesText.text += "\n"; // Space between sections
                _namesText.text += "\n";
            }
        }

        private void _ResetCreditsPosition()
        {
            _creditsTransform.anchoredPosition = new Vector2(_creditsTransform.anchoredPosition.x, 0);
        }
    }
}
