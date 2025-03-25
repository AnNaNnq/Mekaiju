using System.Collections;
using Mekaiju.AI;
using Mekaiju.Entity;
using Mekaiju.AI.Body;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Mekaiju
{
    [Serializable]
    public class DashAlterPayload
    {
        public float forceFactor;
        public float durationFactor;
    }

    public class DashAbility : IStaminableAbility
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
        private float _duration;

        /// <summary>
        /// 
        /// </summary>
        [Header("Visual Effects")]
        [SerializeField]
        private MeshTrailConfig _meshTrailConfig;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _speedVfx;

        /// <summary>
        /// WASD input
        /// </summary>
        [Header("Input")]
        [SerializeField]
        private InputActionReference _input;

        private float _elapedTime;
        private float _endTriggerTimout = 1f;
        private float _actionTriggerTimout = 1f;

        private float _runtimeForce;
        private float _runtimeDuration;

        private Vector3      _direction;
        private MeshTrailTut _ghost;
        private GameObject   _camera;

        private AnimationState _animationState;
        private MechaAnimatorProxy _animationProxy;

        private Rigidbody    _rigidbody;

        public override void Initialize(EntityInstance p_self)
        {
            base.Initialize(p_self);
            
            _elapedTime = 0;

            _runtimeForce    = _force;
            _runtimeDuration = _duration;

            _ghost = p_self.parent.gameObject.AddComponent<MeshTrailTut>();
            _ghost.config = _meshTrailConfig;

            _camera = GameObject.FindGameObjectWithTag("MainCamera");


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

            _animationProxy.onDash.AddListener(_OnDashAnimationEvent);
        }

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return (
                base.IsAvailable(p_self, p_opt) &&
                !p_self.states[StateKind.Protected] &&
                 p_self.states[StateKind.Grounded]  &&
                Mathf.Abs(_input.action.ReadValue<Vector2>().magnitude) > 0    
            );
        }

        public override void Alter(object p_payload)
        {
            if (p_payload is DashAlterPayload t_casted)
            {
                _runtimeForce    = _force    * t_casted.forceFactor;
                _runtimeDuration = _duration * t_casted.durationFactor;
            }
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                Vector2   t_input     = _input.action.ReadValue<Vector2>();
                Transform t_transform = p_self.parent.transform;
                _direction = (t_input.y * t_transform.forward + t_input.x * t_transform.right).normalized;

                if (Mathf.Abs(_direction.sqrMagnitude) > 0)
                {
                    state.Set(AbilityState.Active);
                    _animationState = AnimationState.Idle;

                    ConsumeStamina(p_self);

                    p_self.states[StateKind.MovementOverrided].Set(true);

                    _ghost.Trigger(_runtimeDuration);
                    var t_go = GameObject.Instantiate(_speedVfx, _camera.transform);
                    t_go.transform.Translate(new(0, 0, 2f));

                    _animationProxy.animator.SetTrigger("Dash");

                    // Wait for animation action
                    float t_timout = _actionTriggerTimout;
                    yield return new WaitUntil(() => _animationState == AnimationState.Trigger || (t_timout -= Time.deltaTime) <= 0);

                    _elapedTime = 0;
                    yield return new WaitUntil(() =>
                    {
                        _elapedTime += Time.deltaTime;
                        return _elapedTime >= _runtimeDuration;
                    });

                    GameObject.Destroy(t_go);
                    p_self.states[StateKind.MovementOverrided].Set(false);

                    yield return WaitForCooldown();

                    state.Set(AbilityState.Ready);
                }
            }

            yield return null;
        }

        public override void FixedTick(EntityInstance p_self)
        {
            if (state == AbilityState.Active)
            {
                Vector3 t_vel = _runtimeForce * (1 - _elapedTime / _runtimeDuration) * _direction;
                _rigidbody.linearVelocity = new(t_vel.x, _rigidbody.linearVelocity.y, t_vel.z);
            }   
        }
        private void _OnDashAnimationEvent(AnimationEvent p_event)
        {
            _animationState = p_event.state;
        }
    }
}
