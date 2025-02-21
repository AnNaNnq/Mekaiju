using System.Collections;
using Mekaiju.AI;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    class SwordAbility : IAbilityBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _damage;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _rateOfFire;
        
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _consumption;

        /// <summary>
        /// Distance in m.
        /// </summary>
        [SerializeField]
        private int _reachDistance;

        /// <summary>
        /// 
        /// </summary>
        private float _lastTriggerTime;

        /// <summary>
        /// 
        /// </summary>
        private float _minTimeBetweenFire => 1f / (_rateOfFire / 60f);

        public override void Initialize(MechaPartInstance p_self)
        {
            _lastTriggerTime = -1000f;
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
                p_self.mecha.context.animationProxy.animator.SetTrigger("swordAttack");

                p_self.mecha.ConsumeStamina(_consumption);

                // Compute travel time
                var t_tpos = p_target.transform.position;
                var t_dist = Vector3.Distance(p_self.transform.position, t_tpos);
                if (t_dist < _reachDistance)
                {
                    // TODO: remove cast
                    var t_damage = p_self.mecha.context.modifiers[ModifierTarget.Damage].ComputeValue((float)_damage);
                    p_target.TakeDamage((int)t_damage);
                    if (DebugInfo.Instance)
                    {
                        DebugInfo.Instance.SetTempValue(DebugInfo.Instance.Sword, t_damage.ToString(), 1f);
                    }
                }

            }
            yield return null;
        }
    }

}

