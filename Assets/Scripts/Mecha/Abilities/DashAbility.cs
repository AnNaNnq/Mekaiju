using System.Collections;
using Mekaiju.AI;
using UnityEngine;

namespace Mekaiju
{
    public class DashAbility : IAbilityBehaviour
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
        [SerializeField]
        private float _consumption;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private MeshTrailConfig _meshTrailConfig;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _speedVfx;

        /// <summary>
        /// 
        /// </summary>
        private bool _isAcitve;

        /// <summary>
        /// 
        /// </summary>
        private Vector3 _direction;

        /// <summary>
        /// 
        /// </summary>
        private MeshTrailTut _ghost;

        /// <summary>
        /// 
        /// </summary>
        private GameObject _camera;

        /// <summary>
        /// 
        /// </summary>
        private float _elapedTime;

        public override void Initialize(MechaPartInstance p_self)
        {
            _isAcitve   = false;
            _elapedTime = 0;

            _ghost = p_self.mecha.gameObject.AddComponent<MeshTrailTut>();
            _ghost.config = _meshTrailConfig;

            _camera = GameObject.FindGameObjectWithTag("MainCamera");
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            return (
                !_isAcitve && 
                !p_self.mecha.context.isMovementAltered && 
                p_self.mecha.stamina - _consumption >= 0f &&
                Mathf.Abs(p_self.mecha.context.moveAction.ReadValue<Vector2>().magnitude) > 0    
            );
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                Vector2   t_input     = p_self.mecha.context.moveAction.ReadValue<Vector2>();
                Transform t_transform = p_self.mecha.transform;
                _direction = (t_input.y * t_transform.forward + t_input.x * t_transform.right).normalized;

                if (Mathf.Abs(_direction.sqrMagnitude) > 0)
                {
                    p_self.mecha.ConsumeStamina(_consumption);
                    p_self.mecha.context.isMovementOverrided = true;

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
                    p_self.mecha.context.isMovementOverrided = false;
                }
            }

            yield return null;
        }

        public override void FixedTick(MechaPartInstance p_self)
        {
            if (_isAcitve)
            {
                Rigidbody t_rb  = p_self.mecha.context.rigidbody;
                Vector3   t_vel = _force * (1 - _elapedTime / _duration) * _direction;
                t_rb.linearVelocity = new(t_vel.x, t_rb.linearVelocity.y, t_vel.z);
            }   
        }
    }
}
