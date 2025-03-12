using System.Collections;
using Mekaiju.AI;
using Mekaiju.Entity;
using Mekaiju.AI.Body;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mekaiju
{
    public class DashAbility : IAbilityBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [Header("General")]
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
        [SerializeField]
        private float _consumption;

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

        private bool _isAcitve;
        private float _elapedTime;

        private Vector3      _direction;
        private MeshTrailTut _ghost;
        private GameObject   _camera;
        private Rigidbody    _rigidbody;

        public override void Initialize(EntityInstance p_self)
        {
            _isAcitve   = false;
            _elapedTime = 0;

            _ghost = p_self.parent.gameObject.AddComponent<MeshTrailTut>();
            _ghost.config = _meshTrailConfig;

            _camera = GameObject.FindGameObjectWithTag("MainCamera");

            if (p_self.parent.TryGetComponent<Rigidbody>(out var t_rb))
            {
                _rigidbody = t_rb;
            }
            else
            {
                Debug.LogWarning("Unable to find rigidbody on mecha!");
            }
        }

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return (
                base.IsAvailable(p_self, p_opt) &&
                !_isAcitve && 
                !p_self.states[State.Protected] && 
                p_self.stamina - _consumption >= 0f &&
                Mathf.Abs(_input.action.ReadValue<Vector2>().magnitude) > 0    
            );
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
                    p_self.ConsumeStamina(_consumption);
                    p_self.states[State.MovementOverrided] = true;

                    _ghost.Trigger(_duration);
                    var t_go = GameObject.Instantiate(_speedVfx, _camera.transform);
                    t_go.transform.Translate(new(0, 0, 2f));

                    _isAcitve   = true;
                    _elapedTime = 0;
                    yield return new WaitUntil(() => 
                    {
                        _elapedTime += Time.deltaTime;
                        return _elapedTime >= _duration; 
                    });
                    _isAcitve = false;

                    GameObject.Destroy(t_go);
                    p_self.states[State.MovementOverrided] = false;
                }
            }

            yield return null;
        }

        public override void FixedTick(EntityInstance p_self)
        {
            if (_isAcitve)
            {
                Vector3 t_vel = _force * (1 - _elapedTime / _duration) * _direction;
                _rigidbody.linearVelocity = new(t_vel.x, _rigidbody.linearVelocity.y, t_vel.z);
            }   
        }
    }
}
