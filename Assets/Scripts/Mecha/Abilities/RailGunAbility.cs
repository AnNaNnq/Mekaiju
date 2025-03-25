using System.Collections;
using Mekaiju.AI;
using Mekaiju.Entity;
using Mekaiju.AI.Body;
using MyBox;
using UnityEngine;
using Unity.Cinemachine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    public class RailGunAbility : IStaminableAbility
    {
#region Parameters
        /// <summary>
        /// The damage factor (multiply the mecha damage stat)
        /// </summary>
        [Tooltip("The damage factor (multiply the mecha damage stat).")]
        [SerializeField, Range(0f, 5f)]
        private float _damageFactor;

        /// <summary>
        /// The projectil prefab to be thrown (must contain WeaponBullet comp)
        /// </summary>
        [Header("Projectile")]
        [Tooltip("The projectil prefab to be thrown")]
        [SerializeField]
        private GameObject _projectile;

        /// <summary>
        /// The projectil speed in m/s
        /// </summary>
        [Tooltip("The projectil speed in m/s")]
        [SerializeField, PositiveValueOnly]
        private float _projectileSpeed;

        /// <summary>
        /// The impact vfx prefab
        /// </summary>
        [Header("Impact")]
        [Tooltip("The impact vfx")]
        [SerializeField]
        private GameObject _impactVfx;

        /// <summary>
        /// The impact decal prefab (used to project the impact decal)
        /// </summary>
        [Tooltip("The impact decal prefab")]
        [SerializeField]
        private GameObject _impactDecal;

        [SerializeField]
        private string _launchPosition;

#endregion

        private float _endTriggerTimout    = 5f;
        private float _actionTriggerTimout = 5f;
        private float _projectileDestructionTimout = 10f;

        private Transform          _launchTransform;
        private CinemachineCamera  _camera;
        private AnimationState     _animationState;
        private MechaAnimatorProxy _animationProxy;

        public override void Initialize(EntityInstance p_self)
        {
            base.Initialize(p_self);

            _animationProxy = p_self.parent.GetComponentInChildren<MechaAnimatorProxy>();

            if (!_animationProxy)
            {
                Debug.LogWarning("Unable to find animator proxy on mecha!");
            }

            _launchTransform = p_self.parent.transform.FindNested(_launchPosition);
            _camera          = GameObject.FindWithTag("MainCamera").GetComponent<CinemachineCamera>();

            _animationProxy.onRArm.AddListener(_OnAnimationEvent);
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state.Set(AbilityState.Active);
                _animationState = AnimationState.Idle;

                ConsumeStamina(p_self);

                p_self.states[StateKind.MovementLocked].Set(true);

                _animationProxy.animator.SetTrigger("RArm");

                p_self.parent.GetComponent<PlayerController>().SetArmTargeting(true);

                // Wait for animation action
                float t_timout = _actionTriggerTimout;
                yield return new WaitUntil(() => _animationState == AnimationState.Trigger || (t_timout -= Time.deltaTime) <= 0);

                // Setup projectile and launch
                Vector3 t_direction = _camera.transform.forward.normalized;

                Vector3    t_position = _launchTransform.position + (2f * _camera.transform.forward);
                Quaternion t_rotation = Quaternion.LookRotation(t_direction) * Quaternion.Euler(Vector3.up * 90f);
                var t_wb = GameObject.Instantiate(_projectile, t_position, t_rotation).GetComponent<WeaponBullet>();

                t_wb.OnCollide.AddListener(
                    (t_go, t_collision) => 
                    {
                        if (t_collision.collider.gameObject.TryGetComponent<BodyPartObject>(out var t_bpo))
                        {
                            var t_damage = _damageFactor * p_self.statistics[StatisticKind.Damage].Apply<float>(p_self.modifiers[StatisticKind.Damage]);
                            t_bpo.TakeDamage(p_self.parent, t_damage, DamageKind.Direct);
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
                t_wb.Launch(t_direction * _projectileSpeed);
                t_wb.Timout(_projectileDestructionTimout);

                // Wait for animation end
                t_timout = _endTriggerTimout;
                yield return new WaitUntil(() => _animationState == AnimationState.End || (t_timout -= Time.deltaTime) <= 0);

                p_self.parent.GetComponent<PlayerController>().SetArmTargeting(false);
                
                p_self.states[StateKind.MovementLocked].Set(false);

                yield return WaitForCooldown();

                state.Set(AbilityState.Ready);
            }
        }

        private void _OnAnimationEvent(AnimationEvent p_event)
        {
            _animationState = p_event.state;
        }
    }

}
