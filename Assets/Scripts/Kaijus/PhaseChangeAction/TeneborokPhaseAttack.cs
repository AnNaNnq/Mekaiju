using Mekaiju.Attribute;
using Mekaiju.Entity.Effect;
using MyBox;

namespace Mekaiju.AI.PhaseAttack
{
    public class TeneborokPhaseAttack : IPhaseAttack
    {
        [Separator]
        [SOSelector]
        public Effect effect;

        public override void Action()
        {
            base.Action();
            _kaiju.motor.StopKaiju();
            MechaInstance t_mecha = _kaiju.target.GetComponent<MechaInstance>();
            var t_effect = t_mecha.AddEffect(effect);
        }
    }
}
