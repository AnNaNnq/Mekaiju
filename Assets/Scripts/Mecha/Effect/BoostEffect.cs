using System.Collections.Generic;
using Mekaiju;
using Mekaiju.Utils;
using UnityEngine;

namespace Mekaiju
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
        private EnumArray<ModifierTarget, float> _modifiers;

        /// <summary>
        /// 
        /// </summary>
        private EnumArray<ModifierTarget, Modifier> _modifierRefs;

        public override void OnAdd(MechaInstance p_self)
        {
            _modifiers.ForEach((t_key, t_value) => {
                _modifierRefs[t_key] = p_self.context.modifiers[t_key].Add(t_value);
            });
        }

        public override void OnRemove(MechaInstance p_self)
        {
            _modifierRefs.ForEach((t_key, t_ref) => {
                p_self.context.modifiers[t_key].Remove(t_ref);
            });
        }
    }
}
