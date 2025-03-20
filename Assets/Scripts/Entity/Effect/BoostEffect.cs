using System;
using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju.Entity.Effect
{
    /// <summary>
    /// 
    /// </summary>
    public class BoostEffect : IEffectBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private EnumArray<StatisticKind, Modifier> _modifiers;

        /// <summary>
        /// 
        /// </summary>
        private EnumArray<StatisticKind, Modifier> _modifierRefs;

        public override void OnAdd(EntityInstance p_self)
        {
            _modifierRefs = new();
            _modifiers.ForEach((t_key, t_value) => {
                _modifierRefs[t_key] = p_self.modifiers[t_key].Add(t_value.value / 100f, t_value.kind);
            });
        }

        public override void OnRemove(EntityInstance p_self)
        {
            _modifierRefs.ForEach((t_key, t_ref) => {
                p_self.modifiers[t_key].Remove(t_ref);
            });
        }
    }
}
