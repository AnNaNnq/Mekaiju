using System;
using UnityEngine;

namespace Mekaiju.Entity.Effect
{
    [Serializable]
    public class SlownessEffect : IEffectBehaviour
    {
        [SerializeField]
        private int _slownessPercentage;
        private Modifier _speedMod;

        public override void OnAdd(IEntityInstance p_self)
        {
            _speedMod = p_self.modifiers[Statistics.Speed].Add(-1 * ((float)_slownessPercentage / 100));
        }

        public override void OnRemove(IEntityInstance p_self)
        {
            p_self.modifiers[Statistics.Speed].Remove(_speedMod);
        }
    }
}
