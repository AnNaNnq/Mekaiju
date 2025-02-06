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

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Bullet"))
            {
                _ai.TakeDamage(10, gameObject);
            }
        }
    }
}
