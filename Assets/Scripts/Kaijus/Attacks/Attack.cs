using Mekaiju.Utils;
using System.Collections;
using UnityEngine;
using Mekaiju.Entity.Effect;
using MyBox;
using Mekaiju.Entity;
using Mekaiju.Attribute;
using System.Collections.Generic;

namespace Mekaiju.AI.Attack {
    [System.Serializable]
    public abstract class Attack
    {
        public float timeBeforeAttack = 1;
        public float cooldown;
        public float range;
        protected bool canUse;
        protected KaijuInstance _kaiju;

        public bool canMakeDamage = true;

        [ConditionalField(nameof(canMakeDamage))][Indent]
        public float damage = 50;
        [ConditionalField(nameof(canMakeDamage))][Indent]
        public bool blockable;
        [ConditionalField(nameof(canMakeDamage))][Indent]

        Coroutine _atkCoroutine;

        MechaInstance _mecha;

        List<MechaPartInstance> _mechaParts;

        bool blocked = false;

        public virtual void Init()
        {
            canUse = true;
            _mecha = null;
            _mechaParts = new List<MechaPartInstance>();
            blocked = false;
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

        public IEnumerator Lunch(EntityInstance p_kaiju)
        {
            yield return new WaitForSeconds(timeBeforeAttack);
            Active(p_kaiju);
        }

        public virtual void Active(EntityInstance p_kaiju) {
            canUse = false;
            blocked = false;

            if(_kaiju as KaijuInstance != null)
            {
                _kaiju = (KaijuInstance)p_kaiju;
                _kaiju.detector.OnMechaEnter += OnMechEnter;
                _kaiju.detector.OnMechaExit += OnMechExit;
                _kaiju.detector.OnGround += OnGround;
                _kaiju.detector.OnShieldEnter += OnShieldEnter;
                _kaiju.detector.OnShieldExit += OnShiedExit;
            }
        }

        public virtual IEnumerator AttackEnumerator()
        {
            yield return null;
        }

        public void SendDamage(float p_damage, MechaPartInstance p_part, Effect p_effet = null, float p_effetDuration = -1)
        {
            if (p_part != null)
            {
                if (blocked && blockable) return;

                float t_damage = _kaiju.GetRealDamage(p_damage);
                p_part.TakeDamage(_kaiju, t_damage, DamageKind.Direct);
                _kaiju.AddDPS(t_damage);
                _kaiju.UpdateUI();

                if (p_effet != null)
                {
                    p_part.AddEffect(p_effet, p_effetDuration);
                }
            }

            _kaiju.StartCoroutine(Cooldown(_kaiju));
        }

        protected void SendDamage(float p_damage, Effect p_effet = null, float p_effetDuration = -1)
        {
            _mechaParts.ForEach(_part => SendDamage(p_damage, _part, p_effet, p_effetDuration));
        }

        public IEnumerator Cooldown(EntityInstance p_kaiju)
        {
            yield return p_kaiju.StartCoroutine(UtilsFunctions.CooldownRoutine(cooldown, () => canUse = true));
        }

        public virtual void OnAction() { }

        public virtual void OnEnd()
        {
            _kaiju.motor.StartKaiju();
            _kaiju.brain.MakeAction();
            _kaiju.StartCoroutine(UtilsFunctions.CooldownRoutine(cooldown, () => canUse = true));

            _kaiju.detector.OnMechaEnter -= OnMechEnter;
            _kaiju.detector.OnMechaExit -= OnMechExit;
            _kaiju.detector.OnGround -= OnGround;
            _kaiju.detector.OnShieldExit -= OnShiedExit;
            _kaiju.detector.OnShieldEnter -= OnShieldEnter;
        }

        #region Event
        public virtual void OnMechEnter(MechaPartInstance p_mecha)
        {
            _mechaParts.Add(p_mecha);
        }

        public virtual void OnMechExit(MechaPartInstance p_mecha)
        {
            if (_mechaParts.Contains(p_mecha)) _mechaParts.Remove(p_mecha);
        }

        public virtual void OnGround() { }

        public virtual void OnShieldEnter()
        {
            blocked = true;
        }

        public virtual void OnShiedExit()
        {
            blocked = false;
        }

        #endregion
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
