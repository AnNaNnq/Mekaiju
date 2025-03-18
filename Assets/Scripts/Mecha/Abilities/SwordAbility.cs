using System;
using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
using Mekaiju.Entity;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// The sword ability behaviour
    /// </summary>
    class SwordAbility : IAbilityBehaviour
    {
#region Parameters
        /// <summary>
        /// The damage factor (multiply the mecha damage stat)
        /// </summary>
        [Tooltip("The damage factor (multiply the mecha damage stat).")]
        [SerializeField, Range(0f, 5f)]
        private float _damageFactor;

        /// <summary>
        /// The distance in m
        /// </summary>
        [Tooltip("The distance in m.")]
        [SerializeField]
        private int _reachDistance;

        /// <summary>
        /// Stamina consumption
        /// </summary>
        [Tooltip("Stamina consumption")]
        [SerializeField]
        private int _consumption;
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

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return base.IsAvailable(p_self, p_opt) && p_self.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                state = AbilityState.Active;
                _animationState = AnimationState.Idle;

                _animationProxy.animator.SetTrigger("LArm");
                p_self.ConsumeStamina(_consumption);

                p_self.onCollide.AddListener(
                     (t_collision) =>
                     {
                         if (t_collision.gameObject.TryGetComponent<BodyPartObject>(out var t_bpo))
                         {
                             if (t_bpo != p_target && p_target != null)
                             {
                                 t_bpo = p_target;
                             }
                             var t_damage = _damageFactor * p_self.statistics[StatisticKind.Damage].Apply<float>(p_self.modifiers[StatisticKind.Damage]);
                             t_bpo.TakeDamage(t_damage);
                             p_self.onDealDamage.Invoke(t_damage);
                         }
                     }
                 );

                // Wait for animation end
                var t_timout = _endTriggerTimout;
                yield return new WaitUntil(() => _animationState == AnimationState.End || (t_timout -= Time.deltaTime) <= 0);

                state = AbilityState.Ready;
            }
        }

        private void _OnAnimationEvent(AnimationEvent p_event)
        {
            _animationState = p_event.state;
        }
    }

}

