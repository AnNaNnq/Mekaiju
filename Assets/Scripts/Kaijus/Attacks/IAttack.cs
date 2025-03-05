using Mekaiju.Utils;
using System.Collections;
using UnityEngine;
using Mekaiju.Entity.Effect;

namespace Mekaiju.AI.Attack {
    [System.Serializable]
    public abstract class IAttack
    {
        public float cooldown;
        public float range;
        public bool blockable;

        protected bool canUse;

        public float sphereRadius = 4f;
        public float forwardOffset = 7f;
        public LayerMask layerMask;

        public void Init()
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

        public virtual void Active(KaijuInstance kaiju) { canUse = false; }

        public virtual IEnumerator Attack(KaijuInstance kaiju)
        {
            yield return null;
        }

        public MechaInstance GetPlayerInstance(KaijuInstance kaiju)
        {
            // Calculer la position devant l'objet en tenant compte de sa rotation
            Vector3 spherePosition = kaiju.transform.position + kaiju.transform.forward * forwardOffset;

            // D�tection des objets dans la sph�re
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
                float t_damage = p_kaiju.GetRealDamage(p_damage);
                mecha.TakeDamage(t_damage);
                p_kaiju.AddDPS(t_damage);
                p_kaiju.UpdateUI();
                if(p_effet != null)
                {
                    mecha.AddEffect(p_effet, p_effetDuration);
                }
            }
            p_kaiju.StartCoroutine(Cooldown(p_kaiju));
        }

        public IEnumerator Cooldown(KaijuInstance p_kaiju)
        {
           yield return p_kaiju.StartCoroutine(UtilsFunctions.CooldownRoutine(cooldown, () => canUse = true));
        }
    }
}
