using System;
using UnityEngine;

namespace Mekaiju.Entity.Effect
{
    [Serializable]
    public class StaminaEffect : IEffectBehaviour
    {
        [SerializeField, Range(0f, 1f)]
        private float _percentPerSec;

        public override void Tick(IEntityInstance self)
        {
            if (Time.time - self.timePoints[TimePoint.LastAbilityTriggered] > 2f)
            {
                self.RestoreStamina(_percentPerSec * self.baseStamina * Time.deltaTime);
            }
        }
    }
}
