using Mekaiju.AI.Attack.Instance;
using System.Collections;
using UnityEngine;
using Mekaiju.Utils;

namespace Mekaiju.AI.Attack
{
    public class GravitationalZone : MonoBehaviour
    {
        private AbyssalVortex _stat;
        private KaijuInstance _kaiju;

        public void SetUp(KaijuInstance p_kaiju, AbyssalVortex p_stat)
        {
            _kaiju = p_kaiju;
            _stat = p_stat;
            StartCoroutine(rockFall());
        }

        private IEnumerator rockFall()
        {
            yield return new WaitForSeconds(1);
            for (int i = 0; i < _stat.nbRock; i++)
            {
                GameObject t_kaillou = Instantiate(_stat.gameObjectRock, UtilsFunctions.GetRandomPointInCircle(_stat.radius, transform), Quaternion.identity);
                FallRock t_fr = t_kaillou.GetComponent<FallRock>();
                t_fr.SetUp(_stat, _kaiju);
                Destroy(t_kaillou, 10f);
                yield return new WaitForSeconds(0.2f);
            }
            Destroy(gameObject);
        }

        
    }
}
