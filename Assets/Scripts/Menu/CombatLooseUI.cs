using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mekaiju.UI
{
    public class CombatLooseUI : MonoBehaviour
    {
        private void OnEnable()
        {
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }

        public void ReturnMainMenu()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}