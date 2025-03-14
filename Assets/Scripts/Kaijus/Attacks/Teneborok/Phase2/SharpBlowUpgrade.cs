using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class SharpBlowUpgrade : Attack
    {

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);
            _kaiju.animator.AttackAnimation(nameof(SharpBlowUpgrade));
        }

        public override void OnAction()
        {
            base.OnAction();
            SendDamage(damage);
        }

    }
}
