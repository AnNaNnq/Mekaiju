using System.Collections;
using Mekaiju.AI;
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
#endregion

        private float _endTriggerTimout    = 5f;
        private float _actionTriggerTimout = 5f;
        private float _projectileDestructionTimout = 10f;

        private bool _isActive;
        private bool _isAnimationAction;
        private bool _isAnimationEnd;

        public override void Initialize(MechaPartInstance p_self)
        {
            _isActive = false;
            _isAnimationAction = false;
            _isAnimationEnd    = false;
            p_self.mecha.context.animationProxy.onRArm.AddListener(_OnAnimationEvent);
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            return !_isActive && p_self.mecha.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                _isActive = true;

                p_self.mecha.context.animationProxy.animator.SetTrigger("RArm");

                // Wait for animation action
                float t_timout = _actionTriggerTimout;
                yield return new WaitUntil(() => _isAnimationAction || (t_timout -= Time.deltaTime) <= 0);
                _isAnimationAction = false;

                p_self.mecha.ConsumeStamina(_consumption);

                // Setup projectile and launch
                var t_wb = GameObject.Instantiate(_projectile).GetComponent<WeaponBullet>();
                t_wb.transform.position = p_self.transform.position + new Vector3(0, 2.5f, 0) + (10f * p_self.mecha.transform.forward);
                t_wb.OnCollide.AddListener(
                    (t_go, t_collision) => 
                    {
                        if (t_collision.gameObject.TryGetComponent<BodyPartObject>(out var t_bpo))
                        {
                            var t_damage = _damageFactor * p_self.mecha.modifiers[ModifierTarget.Damage].ComputeValue(p_self.mecha.desc.damage);
                            p_target.TakeDamage(t_damage);
                            p_self.onDealDamage.Invoke(t_damage);
                        }
                        GameObject.Destroy(t_go);
                    }
                );
                t_wb.Launch(p_self.transform.forward.normalized * _projectileSpeed, p_self.mecha.transform.forward);
                t_wb.Timout(_projectileDestructionTimout);

                // Wait for animation end
                t_timout = _endTriggerTimout;
                yield return new WaitUntil(() => _isAnimationEnd || (t_timout -= Time.deltaTime) <= 0);
                _isAnimationEnd = false;

                _isActive = false;
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
