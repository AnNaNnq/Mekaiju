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

            _desc.DefaultAbility.Behaviour?.Initialize();
            if (_desc.HasSpecial)
            {
                _desc.SpecialAbility.Behaviour?.Initialize();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_damage"></param>
        public void TakeDamage(float p_damage)
        {
            Health = Mathf.Max(0f, Health - p_damage);
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

    }

}
