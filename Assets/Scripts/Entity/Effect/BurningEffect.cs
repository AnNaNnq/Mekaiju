using System;
using UnityEngine;

namespace Mekaiju.Entity.Effect
{

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BurningEffect : IEffectBehaviour
    {
        [SerializeField]
        private float _damage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public override void Tick(EntityInstance p_self)
        {
            p_self.TakeDamage(_damage * Time.deltaTime);
        }
    }

}
