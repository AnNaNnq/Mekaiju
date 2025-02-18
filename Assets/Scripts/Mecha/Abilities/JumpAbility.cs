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
        private float _cooldown;

        /// <summary>
        /// 
        /// </summary>
        private bool _requested;

        /// <summary>
        /// 
        /// </summary>
        private bool _inCooldown;

        /// <summary>
        /// 
        /// </summary>
        private bool _isAnimationStarted;

        public override void Initialize(MechaPartInstance p_self)
        {
            _requested  = false;
            _inCooldown = false;
            _isAnimationStarted = false;
            p_self.mecha.context.animationProxy.onJump.AddListener(_OnJumpAnimationStarted);
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            return p_self.mecha.context.isGrounded && !_requested && !_inCooldown;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {  
            if (IsAvailable(p_self, p_opt))
            {
                p_self.mecha.context.animationProxy.animator.SetTrigger("Jump");
                yield return new WaitUntil(() =>_isAnimationStarted);
                _isAnimationStarted = false;
                _requested = true;
                yield return new WaitUntil(() => !_requested && !p_self.mecha.context.isGrounded);
                yield return new WaitUntil(() => p_self.mecha.context.isGrounded);
                _inCooldown = true;
                yield return new WaitForSeconds(_cooldown);
                _inCooldown = false;
            }
        }

        public override void FixedTick(MechaPartInstance p_self)
        {
            if (_requested)
            {
                p_self.mecha.context.rigidbody.AddForce(Vector3.up * _force, ForceMode.Impulse);
                _requested = false;
            }
        }

        private void _OnJumpAnimationStarted()
        {
            _isAnimationStarted = true;
        }
    }
}