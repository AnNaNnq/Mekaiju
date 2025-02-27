using System.Collections;
using Mekaiju.AI;
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

        private bool _isActive;

        public override void Initialize(MechaPartInstance p_self)
        {
            _isActive = false;
        }

        public override bool IsAvailable(MechaPartInstance p_self, object p_opt)
        {
            return !_isActive && p_self.mecha.stamina - _consumption >= 0f;
        }

        public override IEnumerator Trigger(MechaPartInstance p_self, BodyPartObject p_target, object p_opt)
        {
            if (IsAvailable(p_self, p_opt))
            {
                p_self.mecha.context.animationProxy.animator.SetTrigger("LArm");

                p_self.mecha.ConsumeStamina(_consumption);

                // TODO: use physics to handle contact
                // Compute distance
                var t_tpos = p_target.transform.position;
                var t_dist = Vector3.Distance(p_self.transform.position, t_tpos);
                if (t_dist < _reachDistance)
                {
                    // Make damage
                    var t_damage = _damageFactor * p_self.mecha.modifiers[ModifierTarget.Damage].ComputeValue(p_self.mecha.desc.damage);
                    p_target.TakeDamage(t_damage);
                    p_self.onDealDamage.Invoke(t_damage);
                }

            }
            yield return null;
        }
    }

}

