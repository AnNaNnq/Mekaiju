using Mekaiju.Entity;

namespace Mekaiju.AI.Attack
{
    public class SharpBlow : Attack
    {
        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);
            _kaiju.animator.AttackAnimation(nameof(SharpBlow));
        }

        public override void OnAction()
        {
            base.OnAction();
            SendDamage(damage);
        }

    }
}
