using System;
using System.Collections;
using Mekaiju.AI;
<<<<<<< HEAD
using Mekaiju.Utils;
using UnityEngine;
using UnityEngine.Events;
=======
using Mekaiju.AI.Body;
using Mekaiju.Utils;
using UnityEngine;
using UnityEngine.Events;
using Mekaiju.Entity;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MechaPartInstance : EntityInstance
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaInstance mecha { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private MechaPartDesc _desc;

        /// <summary>
        /// 
        /// </summary>
<<<<<<< HEAD
        [field: SerializeField]
        public float health { get; private set; }
=======
        private float _health;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_inst"></param>
        /// <param name="p_config"></param>
        public void Initialize(MechaInstance p_inst, MechaPartDesc p_desc)
        {
<<<<<<< HEAD
            mecha   = p_inst;

            _desc  = p_desc;
            health = p_desc.health;
=======
            mecha  = p_inst;
            parent = p_inst;

            _desc   = p_desc;
            _health = baseHealth;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54

            _desc.ability.behaviour?.Initialize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_opt"></param>
        /// <returns></returns>
        public IEnumerator TriggerAbility(BodyPartObject p_target, object p_opt)
        {
            if (_desc.ability.behaviour.IsAvailable(this, p_opt))
            {
                mecha.timePoints[TimePoint.LastAbilityTriggered] = Time.time;
                yield return _desc.ability.behaviour.Trigger(this, p_target, p_opt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReleaseAbility()
        {
            _desc.ability.behaviour.Release();
        }

<<<<<<< HEAD
        private void Update()
=======
        public override void Update()
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
        {
            _desc.ability.behaviour?.Tick(this);
        }

<<<<<<< HEAD
        private void FixedUpdate()
=======
        public override void FixedUpdate()
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
        {
            _desc.ability.behaviour?.FixedTick(this);
        }

<<<<<<< HEAD
#region IEntityInstance implementation

        public override EnumArray<Statistics, ModifierCollection> modifiers => mecha.modifiers;

        public override UnityEvent<float> onTakeDamage => mecha.onTakeDamage;
        public override UnityEvent<float> onDealDamage => mecha.onDealDamage;

        public override bool isAlive => health > 0f;

        public override float baseHealth => _desc.health;

        public override void TakeDamage(float p_damage)
        {
            var t_damage = p_damage - p_damage * modifiers[Statistics.Defense].ComputeValue(mecha.desc.defense);
            
            mecha.timePoints[TimePoint.LastDamage] = Time.time;
            health = Mathf.Max(0f, health - t_damage);

            onTakeDamage.Invoke(t_damage);
        }

        public override void Heal(float p_heal)
        {
            health = Mathf.Min(_desc.health, health + p_heal);
=======
        #region IEntityInstance implementation
        public override float ComputedStatistics(Statistics p_kind)
        {
            return parent.ComputedStatistics(p_kind);
        }

        public override EnumArray<Statistics, ModifierCollection> modifiers => parent.modifiers;

        public override EnumArray<TimePoint, float> timePoints => parent.timePoints;
        public override EnumArray<State,     bool> states     => parent.states;

        public override UnityEvent<float> onTakeDamage => parent.onTakeDamage;
        public override UnityEvent<float> onDealDamage => parent.onDealDamage;

        public override bool isAlive => health > 0f;

        public override float baseHealth => _desc.healthPercent * parent.baseHealth;
        public override float health     => _health;

        public override void Heal(float p_heal)
        {
            _health = Mathf.Min(baseHealth, _health + p_heal);
        }

        public override void TakeDamage(float p_damage)
        {
            var t_damage = p_damage - p_damage * ComputedStatistics(Statistics.Defense);
            timePoints[TimePoint.LastDamage] = Time.time;
            _health = Mathf.Max(0f, _health - t_damage);
            onTakeDamage.Invoke(t_damage);
        }

        public override float baseStamina => parent.baseStamina;
        public override float stamina     => parent.stamina;

        public override void ConsumeStamina(float p_amount)
        {
            parent.ConsumeStamina(p_amount);
        }

        public override void RestoreStamina(float p_amount)
        {
            parent.RestoreStamina(p_amount);
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
        }
    }
#endregion
}
