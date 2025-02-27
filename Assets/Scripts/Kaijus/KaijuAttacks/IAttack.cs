using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI {
    [System.Serializable]
    public abstract class IAttack
    {
        public float cooldown;
        public float range;
        public bool blockable;

        [HideInInspector]
        public bool canUse;
        [ReadOnly]
        public float sphereRadius = 1.5f;
        [ReadOnly]
        public float forwardOffset = 2.5f;
        public LayerMask layerMask;

        public IAttack()
        {
            canUse = true;
        }

        public virtual bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            bool t_return = canUse && kaiju.TargetInRange(range);
            if (otherRange > 0)
            {
                t_return &= !kaiju.TargetInRange(otherRange);
            }
            return t_return;
        }

        public virtual void Active(KaijuInstance kaiju) { }

        public virtual IEnumerator Attack(KaijuInstance kaiju)
        {
            yield return null;
        }

        public MechaInstance GetPlayerInstance(KaijuInstance kaiju)
        {
            // Calculer la position devant l'objet en tenant compte de sa rotation
            Vector3 spherePosition = kaiju.transform.position + kaiju.transform.forward * forwardOffset;

            // Détection des objets dans la sphère
            Collider[] hitColliders = Physics.OverlapSphere(spherePosition, sphereRadius, layerMask);

            foreach (Collider hitCollider in hitColliders)
            {
                return hitCollider.GetComponent<MechaInstance>();
            }

            return null;
        }

        public void SendDamage(float p_damage, KaijuInstance p_kaiju, Effect p_effet = null, float p_effetDuration = -1)
        {
            MechaInstance mecha = GetPlayerInstance(p_kaiju);
            if (mecha != null)
            {
                float t_damage = p_kaiju.stats.dmg * (1 + (p_damage / 100));
                mecha.TakeDamage(t_damage);
                p_kaiju.AddDPS(t_damage);
                p_kaiju.UpdateUI();
                if(p_effet != null)
                {
                    mecha.AddEffect(p_effet, p_effetDuration);
                }
            }
        }
    }
}
