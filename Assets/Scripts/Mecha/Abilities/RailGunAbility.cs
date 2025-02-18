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
        private int _damage;

        /// <summary>
        /// Max bullet throw per minute
        /// </summary>
        [SerializeField]
        private int _rateOfFire;

        /// <summary>
        /// Bullet speed in m/s
        /// </summary>
        [SerializeField]
        private float _projectileSpeed;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private GameObject _projectile;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _consumption;

        /// <summary>
        /// 
        /// </summary>
        private float _lastTriggerTime;

        /// <summary>
        /// 
        /// </summary>
        private float _minTimeBetweenFire => 1f / (_rateOfFire / 60f);

        /// <summary>
        /// 
        /// </summary>
        private bool _isAnimationStarted;

        public override void Initialize(MechaPartInstance p_self)
        {
            _lastTriggerTime = 0f;
            _isAnimationStarted = false;
            p_self.mecha.context.animationProxy.onFire.AddListener(_OnAnimationStarted);
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            return Time.time - _lastTriggerTime >= _minTimeBetweenFire && p_self.mecha.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            var t_now     = Time.time;
            var t_elapsed = t_now - _lastTriggerTime;
            if (t_elapsed >= _minTimeBetweenFire)
            {
                _lastTriggerTime = t_now;
                // TODO: Launch animation
                p_self.mecha.context.animationProxy.animator.SetTrigger("laserAttack");

                yield return new WaitUntil(() => _isAnimationStarted);
                _isAnimationStarted = false;


                p_self.mecha.ConsumeStamina(_consumption);

                var t_targetPosition = p_target.transform.position;

                // Compute travel time
                var t_dist = Vector3.Distance(p_self.transform.position, t_targetPosition);
                var t_time = t_dist / _projectileSpeed;

                bool t_hasCollide = false;

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
                float t_timout = t_time;
                yield return new WaitUntil(() => t_hasCollide || (t_timout -= Time.deltaTime) <= 0);

                // Check new position of BasicAI ?
                if (t_hasCollide)
                {
                    // TODO: remove cast
                    var t_damage = p_self.mecha.context.modifiers[ModifierTarget.Damage].ComputeValue((float)_damage);
                    p_target.TakeDamage((int)t_damage);
                    if (DebugInfo.Instance)
                    {
                        DebugInfo.Instance.SetTempValue(DebugInfo.Instance.Gun, t_damage.ToString(), 0.5f);
                    }
                }
                GameObject.Destroy(t_go);
            }
        }

        private void _OnAnimationStarted()
        {
            _isAnimationStarted = true;
        }
    }

}
