<<<<<<< Updated upstream
<<<<<<< HEAD
using System.Collections;
using Mekaiju.AI;
=======
using System;
using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
=======
using System;
using System.Collections;
using Mekaiju.AI;
using Mekaiju.AI.Body;
>>>>>>> Stashed changes
using Mekaiju.Entity;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
<<<<<<< Updated upstream
<<<<<<< HEAD
    /// 
=======
    /// The sword ability behaviour
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
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
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
=======
        [Tooltip("The distance in m.")]
>>>>>>> Stashed changes
        [SerializeField]
        private int _reachDistance;

        /// <summary>
<<<<<<< Updated upstream
<<<<<<< HEAD
        /// Stamina consumption for a shot
=======
        /// Stamina consumption
>>>>>>> Stashed changes
        /// </summary>
        [Tooltip("Stamina consumption")]
        [SerializeField]
        private int _consumption;
        #endregion

        private float _endTriggerTimout = 10;

        private bool _isActive;

        private AnimationState _animationState;
        private MechaAnimatorProxy _animationProxy;

        public override void Initialize(EntityInstance p_self)
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

            _animationProxy.onLArm.AddListener(_OnAnimationEvent);
        }

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return !_isActive && !p_self.states[State.Stun] && p_self.stamina - _consumption >= 0f;
        }

<<<<<<< Updated upstream
                // Compute travel time
=======
        /// Stamina consumption
        /// </summary>
        [Tooltip("Stamina consumption")]
        [SerializeField]
        private int _consumption;
#endregion

        private float _endTriggerTimout = 10;

        private bool _isActive;

        private AnimationState     _animationState;
        private MechaAnimatorProxy _animationProxy;

        public override void Initialize(EntityInstance p_self)
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

            _animationProxy.onLArm.AddListener(_OnAnimationEvent);
        }

        public override bool IsAvailable(EntityInstance p_self, object p_opt)
        {
            return !_isActive && !p_self.states[State.Stun] && p_self.stamina - _consumption >= 0f;
        }

=======
>>>>>>> Stashed changes
        public override IEnumerator Trigger(EntityInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
<<<<<<< Updated upstream
                _isActive       = true;
=======
                _isActive = true;
>>>>>>> Stashed changes
                _animationState = AnimationState.Idle;

                _animationProxy.animator.SetTrigger("LArm");
                p_self.ConsumeStamina(_consumption);

                // TODO: use physics to handle contact
                // Compute distance
<<<<<<< Updated upstream
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
=======
>>>>>>> Stashed changes
                var t_tpos = p_target.transform.position;
                var t_dist = Vector3.Distance(p_self.transform.position, t_tpos);
                if (t_dist < _reachDistance)
                {
                    // Make damage
<<<<<<< Updated upstream
<<<<<<< HEAD
                    var t_damage = p_self.modifiers[ModifierTarget.Damage].ComputeValue((float)_damage);
                    p_target.TakeDamage((int)t_damage);
=======
                    var t_damage = _damageFactor * p_self.ComputedStatistics(Statistics.Damage);
                    p_target.TakeDamage(t_damage);
>>>>>>> Stashed changes
                    p_self.onDealDamage.Invoke(t_damage);
                }

                // Wait for animation end
                var t_timout = _endTriggerTimout;
                yield return new WaitUntil(() => _animationState == AnimationState.End || (t_timout -= Time.deltaTime) <= 0);

                _isActive = false;
            }
            yield return null;
        }
<<<<<<< Updated upstream
=======
                    var t_damage = _damageFactor * p_self.ComputedStatistics(Statistics.Damage);
                    p_target.TakeDamage(t_damage);
                    p_self.onDealDamage.Invoke(t_damage);
                }

                // Wait for animation end
                var t_timout = _endTriggerTimout;
                yield return new WaitUntil(() => _animationState == AnimationState.End || (t_timout -= Time.deltaTime) <= 0);

                _isActive = false;
            }
            yield return null;
        }
=======
>>>>>>> Stashed changes

        private void _OnAnimationEvent(AnimationEvent p_event)
        {
            _animationState = p_event.state;
        }
<<<<<<< Updated upstream
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
=======
>>>>>>> Stashed changes
    }

}
