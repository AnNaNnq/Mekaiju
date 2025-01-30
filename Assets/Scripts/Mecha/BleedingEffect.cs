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
        private int Damage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public override void Tick(MechaInstance self)
        {
            self.TakeDamage(Damage * Time.deltaTime);
        }
    }

}
