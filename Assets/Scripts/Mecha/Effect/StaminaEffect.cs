using System;
using Mekaiju;
using UnityEngine;

[Serializable]
public class StaminaEffect : IEffectBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float _percentPerSec;

    public override void Tick(MechaInstance self)
    {
        if (Time.time - self.context.lastAbilityTime > 2f)
            self.RestoreStamina(_percentPerSec * self.config.desc.stamina * Time.deltaTime);
    }
}
