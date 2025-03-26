using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.Utils;
using UnityEngine;
using Mekaiju.Entity.Effect;
using Mekaiju.Entity;
using System;

namespace Mekaiju
{

    [Serializable]
    public class BoostPayload : IPayload
    {
        public EnumArray<StatisticKind, float> values;
    }

    /// <summary>
    /// 
    /// </summary>
    public class BoostAbility : IStaminableAbility
    {
        /// <summary>
        /// 
        /// </summary>
        [Header("Effect")]
        [SerializeField]
        private EnumArray<StatisticKind, Modifier> _modifiers;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _duration;

        private EnumArray<StatisticKind, Modifier> _runtimeModifiers;

        public override void Initialize(EntityInstance p_self)
        {
            base.Initialize(p_self);

            _runtimeModifiers = _modifiers;
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state.Set(AbilityState.Active);

                Debug.Log("ddd");

                ConsumeStamina(p_self);

                var t_refs = _runtimeModifiers.Select((t_key, t_value) => {
                    return p_self.modifiers[t_key].Add(t_value.value / 100f, t_value.kind);
                });

                yield return new WaitForSeconds(_duration);

                t_refs.ForEach((t_key, t_ref) => {
                    p_self.modifiers[t_key].Remove(t_ref);
                });

                yield return WaitForCooldown();

                state.Set(AbilityState.Ready);
            }
        }

        public override IAlteration Alter<T>(T p_payload)
        {
            if (p_payload is BoostPayload t_casted)
            {
                BoostPayload t_diff = new();
                t_diff.values = new();

                _runtimeModifiers = _modifiers.Select((t_key, t_mod) => 
                {
                    return new Modifier(_runtimeModifiers[t_key].value + (t_diff.values[t_key] = t_mod.value * t_casted.values[t_key]), t_mod.kind);
                });

                return new Alteration<BoostPayload>(t_casted, t_diff);
            }
            return null;
        }

        public override void Revert(IAlteration p_payload)
        {
            if (p_payload is Alteration<BoostPayload> t_casted)
            {
                _runtimeModifiers = _modifiers.Select((t_key, t_mod) => 
                {
                    return new Modifier(_runtimeModifiers[t_key].value - t_casted.diff.values[t_key], t_mod.kind);
                });
            }
        }
    }
}
