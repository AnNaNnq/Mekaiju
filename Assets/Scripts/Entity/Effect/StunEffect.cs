using System;
using UnityEngine;

namespace Mekaiju.Entity.Effect
{
    /// <summary>
    /// 
    /// </summary>
    public class StunEffect : IEffectBehaviour
    {
        private Guid _lock1, _lock2;

        public override void OnAdd(EntityInstance p_self)
        {
            p_self.states[StateKind.AbilityLocked] .Set(true);
            p_self.states[StateKind.MovementLocked].Set(true);

            _lock1 = p_self.states[StateKind.AbilityLocked] .ForceLock();
            _lock2 = p_self.states[StateKind.MovementLocked].ForceLock();
        }

        public override void OnRemove(EntityInstance p_self)
        {
            if (p_self.states[StateKind.AbilityLocked].IsKeyValid(_lock1))
            {
                p_self.states[StateKind.AbilityLocked] .Unlock(_lock1);
            }
            
            if (p_self.states[StateKind.MovementLocked].IsKeyValid(_lock2))
            {
                p_self.states[StateKind.MovementLocked].Unlock(_lock2);
            }

            p_self.states[StateKind.AbilityLocked] .Set(false);
            p_self.states[StateKind.MovementLocked].Set(false);
        }
    }
}
