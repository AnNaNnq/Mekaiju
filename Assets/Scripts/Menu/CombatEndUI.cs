using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Mekaiju.UI
{
    public class CombatEndUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject _winLabel;
        [SerializeField]
        private GameObject _looseLabel;

        [SerializeField]
        private Button _mainMenuButton;

        private CombatManager _manager;

        private void Awake()
        {
            _winLabel.SetActive(false);
            _looseLabel.SetActive(false);

            _mainMenuButton.onClick.AddListener(_OnMainMenuButton);

            _manager = GameObject.Find("CombatManager").GetComponent<CombatManager>();
        }

        private void OnEnable()
        {
            Time.timeScale = 0;

            switch (_manager.result)
            {
                case CombatResult.Win:
                    _winLabel.SetActive(true);
                    break;
                case CombatResult.Loose:
                    _looseLabel.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }

        private void _OnMainMenuButton()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
