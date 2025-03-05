using Mekaiju.Attribute;
using MyBox;
using Mekaiju.Entity.Effect;
using Mekaiju.Entity;

namespace Mekaiju.AI.Attack
{
    public class BigFangs : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        [SOSelector]
        [OverrideLabel("Effect")]
        public Effect effect;
        [OverrideLabel("Effect duration (sec)")]
        public float effectDuration = 2;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(IEntityInstance kaiju)
        {
            base.Active(kaiju);

            SendDamage(damage, kaiju, effect, effectDuration);
        }
    }
}
