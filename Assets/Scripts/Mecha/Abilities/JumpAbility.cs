using System;
using System.Collections;
using Mekaiju.AI;
using UnityEngine;
using Mekaiju.Entity;

namespace Mekaiju
{
    /// <summary>
    /// 
    /// </summary>
    public class JumpAbility : IAbilityBehaviour
    {
        /// <summary>
        /// The phisycs force applied
        /// </summary>
        [SerializeField]
        private float _force;

        /// <summary>
        /// Time to wait to be able to jump again
        /// </summary>
        [SerializeField]
        private float _cooldown;

        private float _endTriggerTimout    = 5f;
        private float _actionTriggerTimout = 5f;

        private bool _requested;
        private bool _isActive;
        private bool _inCooldown;
        private bool _isAnimationAction;
        private bool _isAnimationEnd;

        public override void Initialize(MechaPartInstance p_self)
        {
            _requested  = false;
            _isActive   = false;
            _inCooldown = false;
            _isAnimationAction = false;
            _isAnimationEnd    = false;
            p_self.mecha.context.animationProxy.onJump.AddListener(_OnJumpAnimationEvent);
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            return (
                !_isActive && 
                 p_self.states[State.Grounded] && 
                !p_self.states[State.Stun] && 
                !_requested && !_inCooldown
            );
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {  
            if (IsAvailable(p_self, p_opt))
            {
                _isActive = true;
                p_self.mecha.context.animationProxy.animator.SetTrigger("Jump");

                // Wait for animation action
                float t_timout = _actionTriggerTimout;
                yield return new WaitUntil(() =>_isAnimationAction || (t_timout -= Time.deltaTime) <= 0);

                _isAnimationAction = false;
                _requested         = true;

                // Wait for physic jump performed
                yield return new WaitUntil(() => !_requested && !p_self.states[State.Grounded]);

                // Wait for jump animation end
                t_timout = _endTriggerTimout;
                yield return new WaitUntil(() => p_self.states[State.Grounded] && (_isAnimationEnd || (t_timout -= Time.deltaTime) <= 0));

                _inCooldown     = true;
                _isAnimationEnd = false;

                // Wait for cooldown
                yield return new WaitForSeconds(_cooldown);

                _inCooldown = false;
                _isActive   = false;
            }
        }

        public override void FixedTick(MechaPartInstance p_self)
        {
            // Perform physic jump if requested
            if (_requested)
            {
                p_self.mecha.context.rigidbody.AddForce(Vector3.up * _force, ForceMode.Impulse);
                _requested = false;
            }
        }

        private void _OnJumpAnimationEvent(AnimationEventType p_eType)
        {
            switch (p_eType)
            {
                case AnimationEventType.Action:
                    _isAnimationAction = true;
                    break;
                case AnimationEventType.End:
                    _isAnimationEnd = true;
                    break;
                default:
                    break;
            }
        }
    }
}