using System.Collections;
using Mekaiju.AI;
using Mekaiju.Entity;
using Mekaiju.AI.Body;
using MyBox;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    public class RailGunAbility : IAbilityBehaviour
    {
#region Parameters
        /// <summary>
        /// The damage factor (multiply the mecha damage stat)
        /// </summary>
        [Tooltip("The damage factor (multiply the mecha damage stat).")]
        [SerializeField, Range(0f, 5f)]
        private float _damageFactor;

        /// <summary>
        /// The projectil speed in m/s
        /// </summary>
        [Tooltip("The projectil speed in m/s")]
        [SerializeField, PositiveValueOnly]
        private float _projectileSpeed;

        /// <summary>
        /// The stamina consumption
        /// </summary>
        [Tooltip("The stamina consumption")]
        [SerializeField, PositiveValueOnly]
        private int _consumption;

        /// <summary>
        /// The projectil prefab to be thrown (must contain WeaponBullet comp)
        /// </summary>
        [Tooltip("The projectil prefab to be thrown")]
        [SerializeField]
        private GameObject _projectile;

        /// <summary>
        /// The impact vfx prefab
        /// </summary>
        [Tooltip("The impact vfx")]
        [SerializeField]
        private GameObject _impactVfx;

        /// <summary>
        /// The impact decal prefab (used to project the impact decal)
        /// </summary>
        [Tooltip("The impact decal prefab")]
        [SerializeField]
        private GameObject _impactDecal;
#endregion

        private float _endTriggerTimout    = 5f;
        private float _actionTriggerTimout = 5f;
        private float _projectileDestructionTimout = 10f;

        private bool _isActive;

        private AnimationState     _animationState;
        private MechaAnimatorProxy _animationProxy;

        public override void Initialize(MechaPartInstance p_self)
        {
            _isActive = false;

            if (p_self.parent.TryGetComponent<MechaAnimatorProxy>(out var t_proxy))
            {
                _animationProxy = t_proxy;
            }
            else
            {
                Debug.LogWarning("Unable to find animator proxy on mecha!");
            }

            _animationProxy.onRArm.AddListener(_OnAnimationEvent);
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            return !_isActive && !p_self.states[State.Stun] && p_self.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                _isActive       = true;
                _animationState = AnimationState.Idle;

                _animationProxy.animator.SetTrigger("RArm");

                // Wait for animation action
                float t_timout = _actionTriggerTimout;
                yield return new WaitUntil(() => _animationState == AnimationState.Trigger || (t_timout -= Time.deltaTime) <= 0);

                p_self.ConsumeStamina(_consumption);

                // Setup projectile and launch
                var t_wb = GameObject.Instantiate(_projectile).GetComponent<WeaponBullet>();
                t_wb.transform.position = p_self.transform.position + new Vector3(0, 2.5f, 0) + (10f * p_self.parent.transform.forward);
                t_wb.OnCollide.AddListener(
                    (t_go, t_collision) => 
                    {
                        if (t_collision.collider.gameObject.TryGetComponent<BodyPartObject>(out var t_bpo))
                        {
                            var t_damage = _damageFactor * p_self.ComputedStatistics(Statistics.Damage);
                            t_bpo.TakeDamage(t_damage);
                            p_self.onDealDamage.Invoke(t_damage);
                        }

                        // Compute tranform for both vfx and decal
                        var t_contact = t_collision.GetContact(0);
                        var t_contactPosition = t_contact.point + t_contact.normal * 0.5f;
                        var t_contactRotation = Quaternion.FromToRotation(Vector3.back, t_contact.normal);

                        // Instanciate vfx
                        var t_vfx = GameObject.Instantiate(_impactVfx, t_contactPosition, t_contactRotation);
                        GameObject.Destroy(t_vfx, 1);

                        // Instanciate decal
                        var t_decal = GameObject.Instantiate(_impactDecal, t_contactPosition, t_contactRotation);
                        GameObject.Destroy(t_decal, 10f);

                        GameObject.Destroy(t_go);
                    }
                );
                t_wb.Launch(p_self.parent.transform.forward.normalized * _projectileSpeed, p_self.parent.transform.forward);
                t_wb.Timout(_projectileDestructionTimout);

                // Wait for animation end
                t_timout = _endTriggerTimout;
                yield return new WaitUntil(() => _animationState == AnimationState.End || (t_timout -= Time.deltaTime) <= 0);

                _isActive = false;
            }
        }

        private void _OnAnimationEvent(AnimationEvent p_event)
        {
            _animationState = p_event.state;
        }
    }

}
