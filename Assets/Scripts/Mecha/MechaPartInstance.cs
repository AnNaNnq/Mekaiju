using System;
using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.Utils;
using UnityEngine;
using UnityEngine.Events;
using Mekaiju.Entity;
using System.Collections.Generic;
using Mekaiju.Entity.Effect;

namespace Mekaiju
{
    using HealthStatisticValue = Mekaiju.Utils.EnumArray<Mekaiju.MechaPart, float>;


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

        /// <summary>
        /// Should be used only by MechaInstance or Self.<br/>
        /// This function is used in case of compound damages.
        /// </summary>
        /// <param name="p_damage">The amount of damage to deal.</param>
        public void TakeDamage(float p_damage)
        {
            _health = Mathf.Max(0f, _health - p_damage);
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
        public override List<StatefullEffect> effects => parent.effects;

        public override UnityEvent<StatefullEffect> onAddEffect    => parent.onAddEffect;
        public override UnityEvent<StatefullEffect> onRemoveEffect => parent.onRemoveEffect;

        public override EnumArray<StatisticKind, ModifierCollection> modifiers   => parent.modifiers;
        public override EnumArray<StatisticKind, IStatistic>        statistics => parent.statistics;

        public override EnumArray<TimePoint, float>  timePoints => parent.timePoints;
        public override EnumArray<StateKind, State> states     => parent.states;

        public override UnityEvent<IDamageable, float, DamageKind> onBeforeTakeDamage => parent.onBeforeTakeDamage;
        public override UnityEvent<IDamageable, float, DamageKind> onAfterTakeDamage  => parent.onAfterTakeDamage;
        public override UnityEvent<float> onDealDamage => parent.onDealDamage;

        public override bool isAlive => health > 0f;

        public override float baseHealth => statistics[StatisticKind.Health].Apply<HealthStatisticValue>(modifiers[StatisticKind.Health])[_desc.part];
        public override float health     => _health;

        public override void Heal(float p_heal)
        {
            _health = Mathf.Min(baseHealth, _health + p_heal);
        }

        public override void TakeDamage(IDamageable p_from, float p_damage, DamageKind p_kind)
        {
            onBeforeTakeDamage.Invoke(p_from, p_damage, p_kind);
            if (!states[StateKind.Invulnerable])
            {
                var t_defense = statistics[StatisticKind.Defense].Apply<float>(modifiers[StatisticKind.Defense]);
                var t_damage  = p_damage - p_damage * t_defense;
                timePoints[TimePoint.LastDamage] = Time.time;
                TakeDamage(t_damage);

                onAfterTakeDamage.Invoke(p_from, t_damage, p_kind);
            }
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
