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
            return p_self.Mecha.Context.IsGrounded && !_requested && p_self.Mecha.Stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {  
            if (IsAvailable(p_self, p_opt))
            {
                p_self.Mecha.ConsumeStamina(_consumption);
                _requested = true;
            }
            yield return null;
        }

        public override void FixedTick(MechaPartInstance p_self)
        {
            if (_requested)
            {
                p_self.Mecha.Context.Animator.SetTrigger("Jump");
                p_self.Mecha.Context.Rigidbody.AddForce(Vector3.up * _force, ForceMode.Impulse);
                _requested = false;
            }
        }
    }
}