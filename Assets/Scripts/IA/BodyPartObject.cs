using UnityEngine;

namespace Mekaiju.AI
{
    public class BodyPartObject : MonoBehaviour
    {
        BasicAI _ai;

        void Start()
        {
            _ai = GetComponentInParent<BasicAI>();
        }

        public void TakeDamage(int p_damage)
        {
            _ai.TakeDamage(p_damage, gameObject);
        }
    }
}
