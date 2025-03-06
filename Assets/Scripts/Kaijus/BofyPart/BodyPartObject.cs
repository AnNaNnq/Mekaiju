using UnityEngine;

namespace Mekaiju.AI.Body
{
    public class BodyPartObject : MonoBehaviour
    {
        KaijuInstance _instance;

        void Start()
        {
            _instance = GetComponentInParent<KaijuInstance>();
        }

        public void TakeDamage(float p_damage)
        {
            Debug.Log("BodyPartObject.TakeDamage() called");
            _instance.TakeDamage(gameObject, p_damage);
        }
    }
}