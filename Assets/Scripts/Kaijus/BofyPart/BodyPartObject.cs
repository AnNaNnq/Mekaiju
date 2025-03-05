using Mekaiju.Entity;
using UnityEngine;

namespace Mekaiju.AI.Body
{
    public class BodyPartObject : MonoBehaviour
    {
        [SerializeField]
        IEntityInstance _instance;

        public bool isOnKaiju = true;

        void Start()
        {
            if(isOnKaiju) _instance = GetComponentInParent<KaijuInstance>();
            else _instance = GetComponent<LittleKaijuInstance>();
        }

        public void TakeDamage(float p_damage)
        {
            if (isOnKaiju) (_instance as KaijuInstance).TakeDamage(gameObject, p_damage);
            else { (_instance as LittleKaijuInstance).TakeDamage(p_damage); Debug.Log("dmg"); }

        }
    }
}
