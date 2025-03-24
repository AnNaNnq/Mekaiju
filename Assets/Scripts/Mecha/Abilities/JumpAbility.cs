using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using UnityEngine;
using Mekaiju.Entity;
using MyBox;
using System;

namespace Mekaiju
{
    [Serializable]
    public class JumpPayload : IPayload
    {
        public float force;
    }

    /// <summary>
    /// 
    /// </summary>
    public class JumpAbility : IStaminableAbility
    {
        /// <summary>
        /// The phisycs force applied
        /// </summary>
        [SerializeField]
        private float _force;

        private float _endTriggerTimout    = 5f;
        private float _actionTriggerTimout = 5f;

        private float _runtimeForce;

        private bool _requested;

        private AnimationState     _animationState;
        private MechaAnimatorProxy _animationProxy;
        private Rigidbody          _rigidbody;

        public override void Initialize(EntityInstance p_self)
        {
            base.Initialize(p_self);
            
            _requested    = false;
            _runtimeForce = _force;

            _animationProxy = p_self.parent.GetComponentInChildren<MechaAnimatorProxy>();

            if (!_animationProxy)
            {
                Debug.LogWarning("Unable to find animator proxy on mecha!");
            }

            if (p_self.parent.TryGetComponent<Rigidbody>(out var t_rb))
            {
                _rigidbody = t_rb;
            }
            else
            {
                Debug.LogWarning("Unable to find rigidbody on mecha!");
            }

            _animationProxy.onJump.AddListener(_OnJumpAnimationEvent);
        }

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return base.IsAvailable(p_self, p_opt) && p_self.states[StateKind.Grounded] && !_requested && _runtimeForce > 0f;
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {  
            if (IsAvailable(p_self, p_opt))
            {
                state = AbilityState.Active;
                _animationState = AnimationState.Idle;

                _animationProxy.animator.SetTrigger("Jump");

                // Wait for animation action
                float t_timout = _actionTriggerTimout;
                yield return new WaitUntil(() => _animationState == AnimationState.Trigger || (t_timout -= Time.deltaTime) <= 0);

                _requested = true;

                // Wait for physic jump performed
                yield return new WaitUntil(() => !_requested && !p_self.states[StateKind.Grounded]);

                // Wait for jump animation end
                t_timout = _endTriggerTimout;
                yield return new WaitUntil(() => p_self.states[StateKind.Grounded] && (_animationState == AnimationState.End || (t_timout -= Time.deltaTime) <= 0));

                yield return WaitForCooldown();

                state = AbilityState.Ready;
            }
        }

        public override IAlteration Alter<T>(T p_payload)
        {
            if (p_payload is JumpPayload t_casted)
            {
                JumpPayload t_diff = new();

                _runtimeForce += (t_diff.force = _force * t_casted.force);

                return new Alteration<JumpPayload>(t_casted, t_diff);
            }
            return null;
        }

        public override void Revert(IAlteration p_payload)
        {
            if (p_payload is Alteration<JumpPayload> t_casted)
            {
                _runtimeForce -= t_casted.diff.force;
            }
        }

        public override void FixedTick(EntityInstance p_self)
        {
            // Perform physic jump if requested
            if (_requested)
            {
                _rigidbody.AddForce(Vector3.up * _runtimeForce, ForceMode.Impulse);
                _requested = false;
            }
        }

        private void _OnJumpAnimationEvent(AnimationEvent p_event)
        {
            _animationState = p_event.state;
        }
    }
}