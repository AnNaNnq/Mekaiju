using Mekaiju.Attribute;
using MyBox;
using Mekaiju.Entity.Effect;
using Mekaiju.Entity;

<<<<<<< HEAD
namespace Mekaiju.AI
=======
namespace Mekaiju.AI.Attack
>>>>>>> 5f85662364b284b3df7b33ea749d4d53e2ca3f54
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

            SendDamage(damage, p_kaiju, effect, effectDuration);
        }
    }
}
