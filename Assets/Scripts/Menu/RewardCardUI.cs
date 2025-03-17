using Mekaiju;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mekaiju.UI
{
    public class RewardCardUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _name;
        [SerializeField]
        private TextMeshProUGUI _description;
        [SerializeField]
        private TextMeshProUGUI _target;

        [SerializeField]
        private Image _icon;

        public UnityEvent onClick;

        public void Awake()
        {
            onClick = new();
        }

        public void Setup(Ability t_ability)
        {
            _name.text        = t_ability.name;
            _description.text = t_ability.description;
            _target.text      = t_ability.granter.targetName;
        }

        public void OnClick()
        {
            onClick.Invoke();
        }
    }
}