using Mekaiju.Utils;
using System.Collections;
using UnityEngine;
using Mekaiju.Entity.Effect;
using MyBox;
using Mekaiju.Entity;
using Mekaiju.Attribute;

namespace Mekaiju.AI.Attack {
    [System.Serializable]
    public abstract class IAttack
    {
        public float cooldown;
        public float range;
        protected bool canUse;

        public bool canMakeDamage = true;

        [ConditionalField(nameof(canMakeDamage))] [Indent]
        public float damage = 50;
        [ConditionalField(nameof(canMakeDamage))] [Indent]
        public bool blockable;
        [ConditionalField(nameof(canMakeDamage))] [Indent]
        public float sphereRadius = 4f;
        [ConditionalField(nameof(canMakeDamage))] [Indent]
        public float forwardOffset = 7f;
        [ConditionalField(nameof(canMakeDamage))] [Indent]
        public LayerMask layerMask;

        public void Init()
        {
            canUse = true;
        }

        public virtual bool CanUse(KaijuInstance p_kaiju, float p_otherRange = 0)
        {
            bool t_return = canUse && p_kaiju.TargetInRange(range);
            if (p_otherRange > 0)
            {
                t_return &= !p_kaiju.TargetInRange(p_otherRange);
            }
            return t_return;
        }


        public bool CanUse(float p_range, float p_otherRange = 0)
        {
            bool t_return = canUse && p_range <= range;
            if (p_otherRange > 0)
            {
                t_return &= !(p_otherRange <= range);
            }
            return t_return;
        }

        public virtual void Active(EntityInstance p_kaiju) { canUse = false; }

        public virtual IEnumerator Attack(EntityInstance p_kaiju)
        {
            yield return null;
        }

        public MechaInstance GetPlayerInstance(EntityInstance p_kaiju)
        {
            // Calculer la position devant l'objet en tenant compte de sa rotation
            Vector3 spherePosition = p_kaiju.transform.position + p_kaiju.transform.forward * forwardOffset;

            // D�tection des objets dans la sph�re
            Collider[] hitColliders = Physics.OverlapSphere(spherePosition, sphereRadius, layerMask);

            foreach (Collider hitCollider in hitColliders)
            {
                return hitCollider.GetComponent<MechaInstance>();
            }
            

            return null;
        }

        public void SendDamage(float p_damage, EntityInstance p_kaiju, Effect p_effet = null, float p_effetDuration = -1)
        {
            MechaInstance mecha = GetPlayerInstance(p_kaiju);

            if (mecha != null)
            {
                if(p_kaiju as KaijuInstance)
                {
                    KaijuInstance t_kaiju = p_kaiju as KaijuInstance;
                    float t_damage = t_kaiju.GetRealDamage(p_damage);
                    mecha.TakeDamage(t_damage);
                    t_kaiju.AddDPS(t_damage);
                    t_kaiju.UpdateUI();
                }
                
                if(p_effet != null)
                {
                    mecha.AddEffect(p_effet, p_effetDuration);
                }
            }

            p_kaiju.StartCoroutine(Cooldown(p_kaiju));
        }

        public IEnumerator Cooldown(EntityInstance p_kaiju)
        {
           yield return p_kaiju.StartCoroutine(UtilsFunctions.CooldownRoutine(cooldown, () => canUse = true));
        }
    }
}
