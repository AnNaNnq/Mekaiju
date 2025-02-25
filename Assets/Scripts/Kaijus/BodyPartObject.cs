using UnityEngine;

namespace Mekaiju.AI
{
    public class BodyPartObject : MonoBehaviour
    {
        KaijuInstance _instance;

        void Start()
        {
            _instance = GetComponentInParent<KaijuInstance>();
        }

        public void TakeDamage(int p_damage)
        {
            _instance.TakeDamage(gameObject, p_damage);
        }
    }
}
