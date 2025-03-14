using Mekaiju.Attribute;
using MyBox;
using Mekaiju.Entity.Effect;
using Mekaiju.Entity;

namespace Mekaiju.AI.Attack
{
    public class BigFangs : Attack
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
            _kaiju.animator.AttackAnimation("Fang");
        }

        public override void OnAction()
        {
            base.OnAction();

            SendDamage(damage, effect, effectDuration);
        }
    }
}
