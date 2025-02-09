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
        private bool _isAcitve;

        /// <summary>
        /// 
        /// </summary>
        private Vector3 _direction;

        public override void Initialize()
        {
            _isAcitve = false;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (!_isAcitve && !p_self.Mecha.Context.IsMovementAltered)
            {
                Vector2   t_input     = p_self.Mecha.Context.MoveInput;
                Transform t_transform = p_self.Mecha.transform;
                _direction = (t_input.y * t_transform.forward + t_input.x * t_transform.right).normalized;

                if (Mathf.Abs(_direction.sqrMagnitude) > 0)
                {
                    p_self.Mecha.ConsumeStamina(_consumption);
                    p_self.Mecha.Context.IsMovementOverrided = true;

                    _isAcitve = true;
                    yield return new WaitForSeconds(_duration);
                    _isAcitve = false;

                    p_self.Mecha.Context.IsMovementOverrided = false;
                }
            }

            yield return null;
        }

        public override void FixedTick(MechaPartInstance p_self)
        {
            if (_isAcitve)
            {
                Rigidbody t_rb  = p_self.Mecha.Context.Rigidbody;
                Vector3   t_vel = _direction * _force;
                t_rb.linearVelocity = new(t_vel.x, t_rb.linearVelocity.y, t_vel.z);
            }   
        }

        public override float Consumption(object p_opt)
        {
            return _consumption;
        }
    }
}
