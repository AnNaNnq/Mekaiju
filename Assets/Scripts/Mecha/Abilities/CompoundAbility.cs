using System;
using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public abstract class ICompoundAbility : IAbilityBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public abstract void CheckAbilityLoop(Ability parent);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    [Serializable]
    public abstract class CompoundAbility<E> : ICompoundAbility where E : Enum
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        protected EnumArray<E, Ability> _abilities;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public override void CheckAbilityLoop(Ability parent)
        {
            foreach (var ability in _abilities)
            {
                if (ability == parent)
                {
                    Debug.LogWarning($"Be carefull with CompoundAbility. There is an Ability loop in {parent.name}");
                }
            }
        }

        public override void Initialize(MechaPartInstance p_self)
        {
            foreach (var t_ability in _abilities)
            {
                if (t_ability)
                {
                    t_ability.behaviour?.Initialize(p_self);
                }
            };
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            if (typeof(E).IsAssignableFrom(p_opt.GetType()))
            {
                Ability t_ability = _abilities[(E)p_opt];
                if (t_ability)
                {
                    return t_ability.behaviour?.IsAvailable(p_self, null) ?? false;
                }
            }

            return false;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (typeof(E).IsAssignableFrom(p_opt.GetType()))
            {
                Ability t_ability = _abilities[(E)p_opt];
                if (t_ability)
                {
                    yield return t_ability.behaviour?.Trigger(p_self, p_target);
                }
                else
                {
                    Debug.Log($"Ability on selector {Enum.GetName(typeof(E), p_opt)} not set to an instance!");
                    yield return null;
                }
            }
        }

        public override void Tick(MechaPartInstance p_self)
        {
            foreach (var t_ability in _abilities)
            {
                if (t_ability)
                {
                    t_ability.behaviour?.Tick(p_self);
                }
            }
        }

        public override void FixedTick(MechaPartInstance p_self)
        {
            foreach (var t_ability in _abilities)
            {
                if (t_ability)
                {
                    t_ability.behaviour?.FixedTick(p_self);
                }
            }
        }
    }

}
