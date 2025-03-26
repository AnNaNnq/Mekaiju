using MyBox;
using Mekaiju.Entity;

namespace Mekaiju.AI.Attack
{
    public class SharpMandible : Attack
    {
        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            _kaiju.animator.AttackAnimation(nameof(SharpMandible));
        }

        public override void OnAction()
        {
            base.OnAction();
            SendDamage(damage);
        }
    }
}
