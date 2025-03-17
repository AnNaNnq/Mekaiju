using Mekaiju.Attribute;
using Mekaiju.Entity.Effect;
using MyBox;

namespace Mekaiju.AI.Attack
{
    [System.Serializable]
    public class DamageZoneStats
    {
        public float damage;
        public float tickRate = 0.2f;

        public bool addEffect = false;
        [ConditionalField(nameof(addEffect))][Indent] public Effect effect;
        [ConditionalField(nameof(addEffect))][Indent] public bool asDuration = false;

        [ConditionalField(true, nameof(_EffectAndDuration))][Indent(2)] public float effectDuration;
        bool _EffectAndDuration() => addEffect && asDuration;
    }
}
