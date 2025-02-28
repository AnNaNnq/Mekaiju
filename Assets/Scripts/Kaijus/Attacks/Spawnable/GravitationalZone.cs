using Mekaiju.AI;
using System.Collections;
using UnityEngine;

namespace Mekaiju.Attacks
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
                GameObject t_kaillou = Instantiate(_stat.gameObjectRock, GetRandomPointInCircle(_stat.radius), Quaternion.identity);
                FallRock t_fr = t_kaillou.GetComponent<FallRock>();
                t_fr.SetUp(_stat, _kaiju);
                Destroy(t_kaillou, 10f);
                yield return new WaitForSeconds(0.2f);
            }
            Destroy(gameObject);
        }

        Vector3 GetRandomPointInCircle(float radius)
        {
            float angle = Random.Range(0f, Mathf.PI * 2); // Angle aléatoire en radians
            float distance = Mathf.Sqrt(Random.Range(0f, 1f)) * radius; // Rayon aléatoire (évite une distribution biaisée)

            float x = transform.position.x + Mathf.Cos(angle) * distance;
            float z = transform.position.z + Mathf.Sin(angle) * distance;
            float y = transform.position.y + 20; // Hauteur fixée

            return new Vector3(x, y, z);
        }
    }
}
