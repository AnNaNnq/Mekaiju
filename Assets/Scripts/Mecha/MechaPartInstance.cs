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
        private MechaPartDesc _desc;

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
        public void Initialize(MechaInstance p_inst, MechaPartDesc p_config)
        {
            Mecha   = p_inst;

            _desc  = p_config;
            Health = p_config.Health;

            _desc.DefaultAbility.Behaviour?.Initialize(this);
            if (_desc.HasSpecial)
            {
                _desc.SpecialAbility.Behaviour?.Initialize(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_damage"></param>
        public void TakeDamage(float p_damage)
        {
            Mecha.Context.LastDamageTime = Time.time;
            Health = Mathf.Max(0f, Health - p_damage);
        }

        public void Heal(float p_heal)
        {
            Health = Mathf.Min(_desc.Health, Health + p_heal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_opt"></param>
        /// <returns></returns>
        public IEnumerator TriggerDefaultAbility(BodyPartObject p_target, object p_opt)
        {
            if (Mecha.CanExecuteAbility(_desc.DefaultAbility.Behaviour.Consumption(p_opt)))
            {
                Mecha.Context.LastAbilityTime = Time.time;
                yield return _desc.DefaultAbility.Behaviour.Trigger(this, p_target, p_opt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_opt"></param>
        /// <returns></returns>
        public IEnumerator TriggerSpecialAbility(BodyPartObject p_target, object p_opt)
        {   
            if (_desc.HasSpecial)
            {
                if (Mecha.CanExecuteAbility(_desc.SpecialAbility.Behaviour.Consumption(p_opt)))
                {
                    Mecha.Context.LastAbilityTime = Time.time;
                    yield return _desc.SpecialAbility.Behaviour.Trigger(this, p_target, p_opt);    
                }
            }
        }

        private void Update()
        {
            _desc.DefaultAbility.Behaviour?.Tick(this);
            if (_desc.SpecialAbility)
            {
                _desc.SpecialAbility.Behaviour?.Tick(this);
            }
        }

        private void FixedUpdate()
        {
            _desc.DefaultAbility.Behaviour?.FixedTick(this);
            if (_desc.SpecialAbility)
            {
                _desc.SpecialAbility.Behaviour?.FixedTick(this);
            }
        }

    }

}
