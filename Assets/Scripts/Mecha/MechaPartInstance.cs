using System;
using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.Utils;
using UnityEngine;
using UnityEngine.Events;
using Mekaiju.Entity;

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
        private float _health;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_inst"></param>
        /// <param name="p_config"></param>
        public void Initialize(MechaInstance p_inst, MechaPartDesc p_desc)
        {
            mecha  = p_inst;
            parent = p_inst;

            _desc   = p_desc;
            _health = baseHealth;

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

        public override void Update()
        {
            _desc.ability.behaviour?.Tick(this);
        }

        public override void FixedUpdate()
        {
            _desc.ability.behaviour?.FixedTick(this);
        }

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
        }
    }
#endregion
}
