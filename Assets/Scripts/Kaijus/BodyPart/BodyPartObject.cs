using Mekaiju.Entity;
using UnityEngine;

namespace Mekaiju.AI.Body
{
    public class BodyPartObject : MonoBehaviour
    {
        [SerializeField]
        KaijuInstance _instance;

        void Start()
        {
            _instance = GetComponentInParent<KaijuInstance>();
            if (_instance == null)
            {
                _instance = GetComponent<KaijuInstance>();
            }
        }

        public void TakeDamage(float p_damage)
        {
            _instance.TakeDamage(gameObject, p_damage);
        }
    }
}