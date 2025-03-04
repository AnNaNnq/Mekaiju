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
        private EnumArray<ModifierTarget, float> _modifiers;

        /// <summary>
        /// 
        /// </summary>
        private EnumArray<ModifierTarget, Modifier> _modifierRefs;

        public override void OnAdd(IEntityInstance p_self)
        {
            _modifierRefs = new();
            _modifiers.ForEach((t_key, t_value) => {
                _modifierRefs[t_key] = p_self.modifiers[t_key].Add(t_value);
            });
        }

        public override void OnRemove(IEntityInstance p_self)
        {
            _modifierRefs.ForEach((t_key, t_ref) => {
                p_self.modifiers[t_key].Remove(t_ref);
            });
        }
    }
}
