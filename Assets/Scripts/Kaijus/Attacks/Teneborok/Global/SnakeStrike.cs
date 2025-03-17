using Mekaiju.Entity;

namespace Mekaiju.AI.Attack
{
    public class SnakeStrike : Attack
    {

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);
            _kaiju.animator.AttackAnimation(nameof(SnakeStrike));
        }

        public override void OnAction()
        {
            base.OnAction();
            SendDamage(damage);
        }
    }
}
