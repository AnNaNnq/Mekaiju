using System;
using UnityEngine;

namespace Mekaiju.Entity.Effect
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class HealEffect : IEffectBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private int _heal;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private float _timeBeforeHeal = 5f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public override void Tick(IEntityInstance self)
        {
            if (Time.time - self.timePoints[TimePoint.LastDamage] > _timeBeforeHeal)
            {
                self.Heal(_heal * Time.deltaTime);
            }
        }
    }
}
