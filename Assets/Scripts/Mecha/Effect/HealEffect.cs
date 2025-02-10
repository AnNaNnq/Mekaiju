using System;
using UnityEngine;

namespace Mekaiju
{
    [Serializable]
    public class HealEffect : IEffectBehaviour
    {
        [SerializeField]
        private int _heal;

        public float timeBeforeHeal = 5f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public override void Tick(MechaInstance self)
        {
            if (Time.time - self.Context.LastDamageTime > timeBeforeHeal)
                self.Heal(_heal * Time.deltaTime);
        }
    }
}
