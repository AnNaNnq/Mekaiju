using System.Collections;
using Mekaiju.AI;
using UnityEngine;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    public class JumpAbility : IAbilityBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _force;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _consumption;

        /// <summary>
        /// 
        /// </summary>
        private bool _requested;

        public override void Initialize(MechaPartInstance p_self)
        {
            _requested = false;
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            return p_self.mecha.context.isGrounded && !_requested && p_self.mecha.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {  
            if (IsAvailable(p_self, p_opt))
            {
                p_self.mecha.ConsumeStamina(_consumption);
                p_self.mecha.context.animationProxy.animator.SetTrigger("Jump");
                _requested = true;
            }
            yield return null;
        }

        public override void FixedTick(MechaPartInstance p_self)
        {
            if (_requested)
            {
                p_self.mecha.context.rigidbody.AddForce(Vector3.up * _force, ForceMode.Impulse);
                _requested = false;
            }
        }
    }
}