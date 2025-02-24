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

        public override void OnAdd(IEntityInstance p_self)
        {
            _speedMod = p_self.modifiers[ModifierTarget.Speed].Add(-1 * ((float)_slownessPercentage / 100));
        }

        public override void OnRemove(IEntityInstance p_self)
        {
            p_self.modifiers[ModifierTarget.Speed].Remove(_speedMod);
        }
    }
}
