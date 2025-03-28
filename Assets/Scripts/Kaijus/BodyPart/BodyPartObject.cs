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

        public void TakeDamage(IDamageable p_from, float p_damage, DamageKind p_kind)
        {
            _instance.TakeDamage(gameObject, p_from, p_damage, p_kind);
        }
    }
}