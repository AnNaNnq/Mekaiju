using System;
using UnityEngine;

namespace Mekaiju
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BleedingEffect : IEffectBehaviour
    {
        [SerializeField]
        private int _damage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public override void Tick(IEntityInstance p_self)
        {
            p_self.TakeDamage(_damage * Time.deltaTime);
        }
    }

}
