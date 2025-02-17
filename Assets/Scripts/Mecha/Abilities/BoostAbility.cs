using System;
using System.Collections;
using System.Collections.Generic;
using Mekaiju.AI;
using Mekaiju.Utils;
using UnityEngine;

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
        private IDisposable _effectRef;

        /// <summary>
        /// 
        /// </summary>
        private bool _isAcive;

        public override void Initialize(MechaPartInstance p_self)
        {
            _isAcive   = false;
            _effectRef = null;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (!_isAcive)
            {
                p_self.Mecha.ConsumeStamina(_consumption);

                _isAcive = true;
                _effectRef = p_self.Mecha.AddEffect(_boostEffect);
                yield return new WaitForSeconds(_duration);
                p_self.Mecha.RemoveEffect(_effectRef);
                _isAcive = false;
            }
        }

        public override float Consumption(object p_opt)
        {
            return _consumption;
        }
    }
}
