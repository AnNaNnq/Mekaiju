using Mekaiju.Entity;
using Unity.VisualScripting;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class RollAndSlash : Attack
    {
        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)p_kaiju;
            t_kaiju.animator.AttackAnimation(nameof(RollAndSlash));
            t_kaiju.motor.StopKaiju();
            _kaiju = t_kaiju;
        }
    }
}
