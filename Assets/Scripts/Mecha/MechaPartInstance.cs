using System;
using System.Collections;
using Mekaiju.AI;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MechaPartInstance : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public MechaInstance mecha { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private MechaPartConfig _config;

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
        public void Initialize(MechaInstance p_inst, MechaPartConfig p_config)
        {
            mecha   = p_inst;

            _config = p_config;
            health = p_config.desc.health;

            _config.ability.behaviour?.Initialize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_damage"></param>
        public void TakeDamage(float p_damage)
        {
            var t_damage = mecha.context.modifiers[ModifierTarget.Defense].ComputeValue(p_damage);
            
            mecha.timePoints[TimePoint.LastDamage] = Time.time;
            health = Mathf.Max(0f, health - t_damage);
        }

        public void Heal(float p_heal)
        {
            health = Mathf.Min(_config.desc.health, health + p_heal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_opt"></param>
        /// <returns></returns>
        public IEnumerator TriggerAbility(BodyPartObject p_target, object p_opt)
        {
            if (_config.ability.behaviour.IsAvailable(this, p_opt))
            {
                mecha.timePoints[TimePoint.LastAbilityTriggered] = Time.time;
                yield return _config.ability.behaviour.Trigger(this, p_target, p_opt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReleaseAbility()
        {
            _config.ability.behaviour.Release();
        }

        private void Update()
        {
            _config.ability.behaviour?.Tick(this);
        }

        private void FixedUpdate()
        {
            _config.ability.behaviour?.FixedTick(this);
        }

    }

}
