using Mekaiju.Attribute;
using MyBox;
using Mekaiju.Entity.Effect;
using Mekaiju.Entity;

namespace Mekaiju.AI.Attack
{
    public class BigFangs : IAttack
    {
        [Separator]
        [SOSelector]
        [OverrideLabel("Effect")]
        public Effect effect;
        [OverrideLabel("Effect duration (sec)")]
        public float effectDuration = 2;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            SendDamage(damage, p_kaiju, effect, effectDuration);
        }
    }
}
