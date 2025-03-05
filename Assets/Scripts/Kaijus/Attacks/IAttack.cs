using Mekaiju.Utils;
using System.Collections;
using UnityEngine;
using Mekaiju.Entity.Effect;
using MyBox;
using Mekaiju.Entity;

namespace Mekaiju.AI.Attack {
    [System.Serializable]
    public abstract class IAttack
    {
        public float cooldown;
        public float range;
        protected bool canUse;

        public bool canMakeDamage = true;

        [ConditionalField(nameof(canMakeDamage))]
        public bool blockable;
        [ConditionalField(nameof(canMakeDamage))]
        public float sphereRadius = 4f;
        [ConditionalField(nameof(canMakeDamage))]
        public float forwardOffset = 7f;
        [ConditionalField(nameof(canMakeDamage))]
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


        public bool CanUse(float p_range, float otherRange = 0)
        {
            bool t_return = canUse && p_range <= range;
            if (otherRange > 0)
            {
                t_return &= !(otherRange <= range);
            }
            return t_return;
        }

        public virtual void Active(IEntityInstance kaiju) { canUse = false; }

        public virtual IEnumerator Attack(IEntityInstance kaiju)
        {
            yield return null;
        }

        public MechaInstance GetPlayerInstance(IEntityInstance kaiju)
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

        public void SendDamage(float p_damage, IEntityInstance p_kaiju, Effect p_effet = null, float p_effetDuration = -1)
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
                else if (p_kaiju as LittleKaijuInstance)
                {
                    LittleKaijuInstance t_little = p_kaiju as LittleKaijuInstance;
                    float t_damage = t_little.GetRealDamage(p_damage);
                    mecha.TakeDamage(t_damage);
                }
                
                if(p_effet != null)
                {
                    mecha.AddEffect(p_effet, p_effetDuration);
                }
            }

            p_kaiju.StartCoroutine(Cooldown(p_kaiju));
        }

        public IEnumerator Cooldown(IEntityInstance p_kaiju)
        {
           yield return p_kaiju.StartCoroutine(UtilsFunctions.CooldownRoutine(cooldown, () => canUse = true));
        }
    }
}
