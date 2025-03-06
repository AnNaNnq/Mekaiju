using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class SharpBlowUpgrade : IAttack
    {
        [Separator]
        public float timeBeforeAttack = 1;
        [OverrideLabel("Second attack damage (% of DMG)")]
        public float secondDamage = 50;
        public float timeBeforeSecondAttack = 1; 

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(IEntityInstance kaiju)
        {
            base.Active(kaiju);
            kaiju.StartCoroutine(Attack(kaiju));
        }

        public override IEnumerator Attack(IEntityInstance kaiju)
        {
            base.Attack(kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)kaiju;

            t_kaiju.animator.AttackAnimation(nameof(SharpBlowUpgrade));
            yield return new WaitForSeconds(timeBeforeAttack);
            SendDamage(damage, kaiju);

            yield return new WaitForSeconds(timeBeforeSecondAttack);
            SendDamage(secondDamage, kaiju);
            t_kaiju.brain.MakeAction();
        }

    }
}
