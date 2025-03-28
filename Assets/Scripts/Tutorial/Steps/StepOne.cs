using System;
using Mekaiju.AI.Attack;
using Mekaiju.AI.Objet;
using Mekaiju.Entity.Effect;

namespace Mekaiju.Tuto
{
    public class StepOne : ITutorial
    {
        public Effect stun;
        public KaijuAttack attack;

        private IDisposable _effect;

        public void Execute(MechaInstance p_mecha, TutorialInstance p_instance)
        {
            _effect = p_mecha.AddEffect(stun);
            attack.attack.Active(p_instance);
        }

        public void End(MechaInstance p_mecha)
        {
            p_mecha.RemoveEffect(_effect);
        }
    }
}
