using Mekaiju.Entity;

namespace Mekaiju.AI.Attack
{
    public class GroundStrike : ChockWave
    {
        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);
            _kaiju.animator.AttackAnimation(nameof(GroundStrike));
        }

        public override void OnAction()
        {
            base.OnAction();
            LunchWave();
        }
    }
}
