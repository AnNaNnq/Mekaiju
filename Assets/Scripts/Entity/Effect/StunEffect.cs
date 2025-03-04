using UnityEngine;

namespace Mekaiju.Entity.Effect
{
    /// <summary>
    /// 
    /// </summary>
    public class StunEffect : IEffectBehaviour
    {
        public override void OnAdd(IEntityInstance p_self)
        {
            p_self.states[State.Stun] = true;
        }

        public override void OnRemove(IEntityInstance p_self)
        {
            p_self.states[State.Stun] = false;
        }
    }
}
