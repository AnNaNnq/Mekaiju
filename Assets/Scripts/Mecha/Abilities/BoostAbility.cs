using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.Utils;
using UnityEngine;
using Mekaiju.Entity.Effect;
using Mekaiju.Entity;

namespace Mekaiju
{
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
        private Effect _boostEffect;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _duration;

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state.Set(AbilityState.Active);

                ConsumeStamina(p_self);

                p_self.AddEffect(_boostEffect, _duration);
                yield return new WaitForSeconds(_duration);

                yield return WaitForCooldown();

                state.Set(AbilityState.Ready);
            }
        }
    }
}
