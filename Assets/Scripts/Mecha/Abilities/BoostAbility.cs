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
    public class BoostAbility : IAbilityBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private Effect _boostEffect;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _duration;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _consumption;

        /// <summary>
        /// 
        /// </summary>
        private bool _isActive;

        public override void Initialize(EntityInstance p_self)
        {
            _isActive   = false;
        }

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return !_isActive && p_self.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                p_self.ConsumeStamina(_consumption);

                _isActive = true;
                p_self.AddEffect(_boostEffect, _duration);
                yield return new WaitForSeconds(_duration);
                _isActive = false;
            }
        }
    }
}
