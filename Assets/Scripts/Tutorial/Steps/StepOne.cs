using System;
using Mekaiju.Entity.Effect;

namespace Mekaiju.Tuto
{
    public class StepOne : ITutorial
    {
        public Effect stun;

        private IDisposable _effect;

        public void Execute(MechaInstance p_mecha)
        {
            _effect = p_mecha.AddEffect(stun);
        }

        public void End(MechaInstance p_mecha)
        {
            p_mecha.RemoveEffect(_effect);
        }
    }
}
