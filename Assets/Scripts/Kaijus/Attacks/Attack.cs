using Mekaiju.Utils;
using System.Collections;
using UnityEngine;
using Mekaiju.Entity.Effect;
using MyBox;
using Mekaiju.Entity;
using Mekaiju.Attribute;
using TMPro;

namespace Mekaiju.AI.Attack {
    [System.Serializable]
    public abstract class Attack
    {
        public float cooldown;
        public float range;
        protected bool canUse;
        protected KaijuInstance _kaiju;

        public bool canMakeDamage = true;

        [ConditionalField(nameof(canMakeDamage))] [Indent]
        public float damage = 50;
        [ConditionalField(nameof(canMakeDamage))] [Indent]
        public bool blockable;
        [ConditionalField(nameof(canMakeDamage))] [Indent]
        [ReadOnly]
        public float sphereRadius = 4f;
        [ConditionalField(nameof(canMakeDamage))] [Indent]
        [ReadOnly]
        public float forwardOffset = 7f;
        [ConditionalField(nameof(canMakeDamage))] [Indent]
        public LayerMask layerMask;

        Coroutine _atkCoroutine;

        MechaInstance _mecha;

        public void Init()
        {
            canUse = true;
            _mecha = null;
            StopAttackCoroutine();
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

        public virtual void Active(EntityInstance p_kaiju) { 
            canUse = false;
            _kaiju = (KaijuInstance) p_kaiju;
            _kaiju.detector.OnMechaEnter += OnMechEnter;
            _kaiju.detector.OnMechaExit += OnMechExit;  
        }

        public virtual IEnumerator AttackEnumerator(EntityInstance p_kaiju)
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

        protected void SendDamage(float p_damage, EntityInstance p_kaiju, Effect p_effet = null, float p_effetDuration = -1)
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

        public void SendDamage(float p_damage, MechaInstance p_mecha, Effect p_effet = null, float p_effetDuration = -1)
        {
            if (p_mecha != null)
            {
                float t_damage = _kaiju.GetRealDamage(p_damage);
                p_mecha.TakeDamage(t_damage);
                _kaiju.AddDPS(t_damage);
                _kaiju.UpdateUI();

                if (p_effet != null)
                {
                    p_mecha.AddEffect(p_effet, p_effetDuration);
                }
            }

            _kaiju.StartCoroutine(Cooldown(_kaiju));
        }

        protected void SendDamage(float p_damage, Effect p_effet = null, float p_effetDuration = -1)
        {
            SendDamage(p_damage, _mecha, p_effet, p_effetDuration);
        }

        public IEnumerator Cooldown(EntityInstance p_kaiju)
        {
           yield return p_kaiju.StartCoroutine(UtilsFunctions.CooldownRoutine(cooldown, () => canUse = true));
        }

        public virtual void OnAction() { }

        public virtual void OnEnd() 
        {
            _kaiju.motor.StartKaiju();
            _kaiju.StartCoroutine(UtilsFunctions.CooldownRoutine(cooldown, () => canUse = true));

            _kaiju.detector.OnMechaEnter -= OnMechEnter;
            _kaiju.detector.OnMechaExit -= OnMechExit;
        }

        public virtual void OnMechEnter(MechaInstance p_mecha)
        {
            _mecha = p_mecha;
            Debug.Log(_mecha.name);
        }

        public virtual void OnMechExit(MechaInstance p_mecha)
        {
            if (p_mecha == _mecha) _mecha = null;
            Debug.Log(p_mecha.name);
        }

        public virtual void StartAttackCoroutine()
        {
            _atkCoroutine = _kaiju.StartCoroutine(AttackCoroutine());
        }

        public virtual void StopAttackCoroutine()
        {
            if(_atkCoroutine != null)
            {
                _kaiju.StopCoroutine(_atkCoroutine);
                _atkCoroutine = null;
            }
        }

        private IEnumerator AttackCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(.1f);
                SendDamage(damage);
            }
        }
    }
}
