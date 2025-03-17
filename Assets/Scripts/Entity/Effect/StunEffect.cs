using UnityEngine;

namespace Mekaiju.Entity.Effect
{
    /// <summary>
    /// 
    /// </summary>
    public class StunEffect : IEffectBehaviour
    {
        public override void OnAdd(EntityInstance p_self)
        {
            p_self.states[State.AbilityLocked]  = true;
            p_self.states[State.MovementLocked] = true;
        }

        public override void OnRemove(EntityInstance p_self)
        {
            p_self.states[State.AbilityLocked]  = false;
            p_self.states[State.MovementLocked] = false;
        }
    }
}
