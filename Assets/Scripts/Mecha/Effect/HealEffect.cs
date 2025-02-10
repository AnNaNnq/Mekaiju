using System;
using UnityEngine;

namespace Mekaiju
{
    [Serializable]
    public class HealEffect : IEffectBehaviour
    {
        [SerializeField]
        private int _heal;

        public bool canHeal = false;
        public float timeBeforeHeal = 5f;
        private float _time = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        public override void Tick(MechaInstance self)
        {
            if (canHeal) self.Heal(_heal * Time.deltaTime);
            CheckIfCanHeal();
        }

        public void CheckIfCanHeal()
        {
            if (canHeal) return;
            _time += Time.deltaTime;
            if(_time >= timeBeforeHeal)
            {
                canHeal = true;
                _time = 0;
            }
            Debug.Log(_time);
        }

        public void SropHeal()
        {
            _time = 0;
            canHeal = false;
        }
    }
}
