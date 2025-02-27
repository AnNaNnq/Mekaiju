using System;
using System.Collections;
using Mekaiju.AI;
using Mekaiju.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MechaPartInstance : IEntityInstance
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
        [field: SerializeField]
        public float health { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_inst"></param>
        /// <param name="p_config"></param>
        public void Initialize(MechaInstance p_inst, MechaPartDesc p_desc)
        {
            mecha   = p_inst;

            _desc  = p_desc;
            health = p_desc.health;

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

        private void Update()
        {
            _desc.ability.behaviour?.Tick(this);
        }

        private void FixedUpdate()
        {
            _desc.ability.behaviour?.FixedTick(this);
        }

#region IEntityInstance implementation

        public override EnumArray<ModifierTarget, ModifierCollection> modifiers => mecha.modifiers;

        public override UnityEvent<float> onTakeDamage => mecha.onTakeDamage;
        public override UnityEvent<float> onDealDamage => mecha.onDealDamage;

        public override bool isAlive => health > 0f;

        public override float baseHealth => _desc.health;

        public override void TakeDamage(float p_damage)
        {
            var t_damage = mecha.context.modifiers[ModifierTarget.Defense].ComputeValue(p_damage);
            
            mecha.timePoints[TimePoint.LastDamage] = Time.time;
            health = Mathf.Max(0f, health - t_damage);

            onTakeDamage.Invoke(t_damage);
        }

        public override void Heal(float p_heal)
        {
            health = Mathf.Min(_desc.health, health + p_heal);
        }
    }
#endregion
}
