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

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            _lastTriggerTime = -1000f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_self"></param>
        /// <param name="p_target"></param>
        /// <returns></returns>
        public override IEnumerator Trigger(MechaPartInstance p_self, BasicAI p_target, object p_opt)
        {
            var t_now     = Time.time; 
            var t_elapsed = t_now - _lastTriggerTime;
            if (t_elapsed >= _minTimeBetweenFire)
            {
                _lastTriggerTime = t_now;
                // TODO: Launch animation

                p_self.Mecha.ConsumeStamina(_consumption);

                // Compute travel time
                var t_tpos = p_target.transform.position;
                var t_dist = Vector3.Distance(p_self.transform.position, t_tpos);
                if (t_dist < _reachDistance)
                {
                    Debug.Log($"Sword damage : {_damage}");   
                }

            }
            yield return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_opt"></param>
        /// <returns></returns>
        public override float Consumption(object p_opt)
        {
            return _consumption;
        }
    }

}

