using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mekaiju.QTE
{
    public class ShowQTE : MonoBehaviour
    {
        public static ShowQTE instance { get; private set; }

        public Image foreground;
        public Image outline;

        public GameObject qteUI;

        public TextMeshProUGUI inputName;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                Debug.LogWarning("More than one instance of ShowQTE found!");
            }
            instance = this;
        }

        private void Start()
        {
            qteUI.SetActive(false);
        }

        public void SetForeground(float p_value)
        {
            foreground.fillAmount = p_value;
        }

        public void SetOutline(float p_value)
        {
            outline.fillAmount = p_value;
        }

        public void SetName(string p_name)
        {
            inputName.text = p_name;
        }
    }
}
