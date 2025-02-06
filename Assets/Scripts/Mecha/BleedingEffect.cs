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
        public override void Tick(MechaInstance self)
        {
            self.TakeDamage(_damage * Time.deltaTime);
        }
    }

}
