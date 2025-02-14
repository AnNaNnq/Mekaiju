using System;
using UnityEngine;

namespace Mekaiju
{
    [Serializable]
    public class SlownessEffect : IEffectBehaviour
    {
        [SerializeField]
        private int _slownessPercentage;
        private Modifier _speedMod;

        public override void OnAdd(MechaInstance p_self)
        {
            _speedMod = p_self.Context.Modifiers[ModifierTarget.Speed].Add(-1 * (_slownessPercentage / 100));
        }

        public override void OnRemove(MechaInstance p_self)
        {
            p_self.Context.Modifiers[ModifierTarget.Speed].Remove(_speedMod);
        }
    }
}
