using Mekaiju.AI;
using System.Collections;
using UnityEngine;

namespace Mekaiju.Attacks
{
    public class GravitationalZone : MonoBehaviour
    {
        private int _dmg;
        private GameObject _rock;
        private float _radius = 10f;
        private int _nbRock = 10;
        private TeneborokAI _ai;

        public void SetUp(TeneborokAI p_ai)
        {
            _dmg = p_ai.abyssalVortexDamage;
            _rock = p_ai.gameObjectRock;
            _radius = p_ai.abyssalVortexRadius;
            _nbRock = p_ai.abyssalVortexNumberOfRock;
            _ai = p_ai;
            StartCoroutine(rockFall());
        }

        private IEnumerator rockFall()
        {
            yield return new WaitForSeconds(1);
            for (int i = 0; i < _nbRock; i++)
            {
                GameObject t_kaillou = Instantiate(_rock, GetRandomPointInCircle(_radius), Quaternion.identity);
                FallRock t_fr = t_kaillou.GetComponent<FallRock>();
                t_fr.SetUp(_ai);
                Destroy(t_kaillou, 10f);
                yield return new WaitForSeconds(0.2f);
            }
            _ai.AttackCooldown();
            _ai.SetLastAttack(TeneborokAttack.AbyssalVortex);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
