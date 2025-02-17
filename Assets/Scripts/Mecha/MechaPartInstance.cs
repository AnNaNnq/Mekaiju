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
        public MechaInstance Mecha { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private MechaPartConfig _config;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField]
        public float Health { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_inst"></param>
        /// <param name="p_config"></param>
        public void Initialize(MechaInstance p_inst, MechaPartConfig p_config)
        {
            Mecha   = p_inst;

            _config = p_config;
            Health = p_config.desc.Health;

            _config.ability.Behaviour?.Initialize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_damage"></param>
        public void TakeDamage(float p_damage)
        {
            var t_damage = Mecha.Context.Modifiers[ModifierTarget.Defense].ComputeValue(p_damage);
            
            Mecha.Context.LastDamageTime = Time.time;
            Health = Mathf.Max(0f, Health - t_damage);
        }

        public void Heal(float p_heal)
        {
            Health = Mathf.Min(_config.desc.Health, Health + p_heal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_opt"></param>
        /// <returns></returns>
        public IEnumerator TriggerAbility(BodyPartObject p_target, object p_opt)
        {
            if (_config.ability.Behaviour.IsAvailable(this, p_opt))
            {
                Mecha.Context.LastAbilityTime = Time.time;
                yield return _config.ability.Behaviour.Trigger(this, p_target, p_opt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReleaseAbility()
        {
            _config.ability.Behaviour.Release();
        }

        private void Update()
        {
            _config.ability.Behaviour?.Tick(this);
        }

        private void FixedUpdate()
        {
            _config.ability.Behaviour?.FixedTick(this);
        }

    }

}
