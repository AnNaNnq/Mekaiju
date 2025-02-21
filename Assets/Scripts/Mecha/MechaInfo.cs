using UnityEngine;
using UnityEngine.UI;


namespace Mekaiju
{
    public class MechaInfo : MonoBehaviour
    {
        private MechaInstance _inst;

        public Image staminaBarUI;

        void Start()
        {
            _inst = GetComponentInParent<MechaInstance>();
        }

        void Update()
        {
            _SetStaminaBar();
        }

        private void _SetStaminaBar()
        {
            staminaBarUI.fillAmount = _inst.stamina / _inst.config.desc.stamina;
        }
    }
}
