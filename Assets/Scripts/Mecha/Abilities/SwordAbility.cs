using System;
using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.Entity;
using UnityEngine;
using UnityEngine.Events;

namespace Mekaiju
{

    /// <summary>
    /// The sword ability behaviour
    /// </summary>
    class SwordAbility : IStaminableAbility
    {
#region Parameters
        /// <summary>
        /// The damage factor (multiply the mecha damage stat)
        /// </summary>
        [Tooltip("The damage factor (multiply the mecha damage stat).")]
        [SerializeField, Range(0f, 5f)]
        private float _damageFactor;
#endregion

        private float _endTriggerTimout = 1;

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

            _animationProxy.onLArm.AddListener(_OnAnimationEvent);
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state.Set(AbilityState.Active);
                _animationState = AnimationState.Idle;

                ConsumeStamina(p_self);

                _animationProxy.animator.SetTrigger("LArm");

                UnityAction<Collider> t_cb = (t_collision) =>
                {
                    if (t_collision.gameObject.TryGetComponent<BodyPartObject>(out var t_bpo))
                    {
                        if (t_bpo != p_target && p_target != null)
                        {
                            t_bpo = p_target;
                        }
                        var t_damage = _damageFactor * p_self.statistics[StatisticKind.Damage].Apply<float>(p_self.modifiers[StatisticKind.Damage]);
                        t_bpo.TakeDamage(p_self.parent, t_damage, DamageKind.Direct);
                        p_self.onDealDamage.Invoke(t_damage);
                    }
                };

                p_self.onCollide.AddListener(t_cb);

                // Wait for animation end
                var t_timout = _endTriggerTimout;
                yield return new WaitUntil(() => _animationState == AnimationState.End || (t_timout -= Time.deltaTime) <= 0);

                p_self.onCollide.RemoveListener(t_cb);

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

