using System;
using UnityEngine;

namespace Mekaiju
{
    [Serializable]
    public class HealEffect : IEffectBehaviour
    {
        [SerializeField]
        private int _heal;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public override void Tick(MechaInstance self)
        {
            self.Heal(_heal * Time.deltaTime);
        }
    }
}
