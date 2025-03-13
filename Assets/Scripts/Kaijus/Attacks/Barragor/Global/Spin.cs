using Mekaiju.Entity;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class Spin : Attack
    {

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            _kaiju.animator.AttackAnimation(nameof(Spin));
            _kaiju.motor.StopKaiju();
        }

    }
}
