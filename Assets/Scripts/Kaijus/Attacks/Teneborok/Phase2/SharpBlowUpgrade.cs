using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class SharpBlowUpgrade : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        public float timeBeforeAttack = 1;
        [OverrideLabel("Second attack damage (% of DMG)")]
        public float secondDamage = 50;
        public float timeBeforeSecondAttack = 1; 

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            kaiju.StartCoroutine(Attack(kaiju));
        }

        public override IEnumerator Attack(KaijuInstance kaiju)
        {
            base.Attack(kaiju);
            kaiju.animator.AttackAnimation(nameof(SharpBlowUpgrade));
            yield return new WaitForSeconds(timeBeforeAttack);
            SendDamage(damage, kaiju);

            yield return new WaitForSeconds(timeBeforeSecondAttack);
            SendDamage(secondDamage, kaiju);
            kaiju.brain.MakeAction();
        }

    }
}
