using System;
using System.Collections;
using Mekaiju.AI;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    public class RailGunAbility : IAbilityBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _damage;

        /// <summary>
        /// Max bullet throw per minute
        /// </summary>
        [SerializeField]
        private int _rateOfFire;

        /// <summary>
        /// Projectile speed in m/s
        /// </summary>
        [SerializeField]
        private float _projectileSpeed;

        /// <summary>
        /// Projectile prefab (must contain WeaponBullet comp)
        /// </summary>
        [SerializeField]
        private GameObject _projectile;

        /// <summary>
        /// Stamina consumption for a shot
        /// </summary>
        [SerializeField]
        private int _consumption;

        private float _lastTriggerTime;
        private float _minTimeBetweenFire => 1f / (_rateOfFire / 60f);
        private float _endTriggerTimout    = 5f;
        private float _actionTriggerTimout = 5f;

        private bool _isActive;
        private bool _isAnimationAction;
        private bool _isAnimationEnd;

        public override void Initialize(MechaPartInstance p_self)
        {
            _lastTriggerTime = 0f;
            _isActive = false;
            _isAnimationAction = false;
            _isAnimationEnd    = false;
            p_self.mecha.context.animationProxy.onRArm.AddListener(_OnAnimationEvent);
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            return !_isActive && Time.time - _lastTriggerTime >= _minTimeBetweenFire && p_self.mecha.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            var t_now     = Time.time;
            var t_elapsed = t_now - _lastTriggerTime;
            if (t_elapsed >= _minTimeBetweenFire)
            {
                _isActive = true;
                _lastTriggerTime = t_now;

                p_self.mecha.context.animationProxy.animator.SetTrigger("RArm");

                // Wait for animation action
                float t_timout = _actionTriggerTimout;
                yield return new WaitUntil(() => _isAnimationAction || (t_timout -= Time.deltaTime) <= 0);
                _isAnimationAction = false;

                p_self.mecha.ConsumeStamina(_consumption);

                // Compute travel time
                var t_dist = Vector3.Distance(p_self.transform.position, p_target.transform.position);
                var t_time = t_dist / _projectileSpeed;

                bool t_hasCollide = false;

                // Setup projectile and launch
                var t_go = GameObject.Instantiate(_projectile);
                var t_wb = t_go.GetComponent<WeaponBullet>();
                t_wb.transform.position = p_self.transform.position + new Vector3(0, 2.5f, 0) + (2 * p_self.mecha.transform.forward);
                t_wb.OnCollide.AddListener(
                    collision => {
                        if (collision.gameObject.TryGetComponent<BodyPartObject>(out var t_bpo))
                        {
                            t_hasCollide = true;
                        }
                    }
                );
                t_wb.Launch(p_self.transform.forward.normalized * _projectileSpeed, p_self.mecha.transform.forward);
                

                // Wait for bullet travel
                t_timout = t_time;
                yield return new WaitUntil(() => t_hasCollide || (t_timout -= Time.deltaTime) <= 0);

                // Make damage if projectile has collide
                if (t_hasCollide)
                {
                    var t_damage = p_self.mecha.context.modifiers[ModifierTarget.Damage].ComputeValue(_damage);
                    // @TODO: wait for parameter to be float
                    p_target.TakeDamage((int)t_damage);
                    if (DebugInfo.Instance)
                    {
                        DebugInfo.Instance.SetTempValue(DebugInfo.Instance.Gun, t_damage.ToString(), 0.5f);
                    }
                }
                GameObject.Destroy(t_go);

                // Wait for animation end
                t_timout = _endTriggerTimout;
                yield return new WaitUntil(() => _isAnimationEnd || (t_timout -= Time.deltaTime) <= 0);

                _isAnimationEnd = false;
                _isActive       = false;
            }
        }

        private void _OnAnimationEvent(AnimationEventType p_eType)
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
