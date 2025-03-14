using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mekaiju.UI
{
    public class CombatRewardUI : MonoBehaviour
    {
        [SerializeField]
        private Transform _abilitiesContainer;
        [SerializeField]
        private GameObject _rewardCardPrefab;

        private CombatManager _manager;

        private void Awake()
        {
            _manager = GameObject.Find("CombatManager").GetComponent<CombatManager>();
        }

        private void OnEnable()
        {
            Time.timeScale = 0;

            _manager.reward.abilities.ForEach(t_ability => {
                GameObject t_go = Instantiate(_rewardCardPrefab, _abilitiesContainer);
                if (t_go.TryGetComponent<RewardCardUI>(out var t_rcu))
                {
                    t_rcu.Setup(t_ability);
                    t_rcu.onClick.AddListener(() => {
                        t_ability.granter.Grant(GameManager.instance.playerData.mechaDesc, t_ability);
                        SceneManager.LoadScene("MainScene");
                    });
                }
            });
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }
    }
}
