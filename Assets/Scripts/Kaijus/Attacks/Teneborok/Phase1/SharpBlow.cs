using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class SharpBlow : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        public float timeBeforeAttack = 1;

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
            kaiju.animator.AttackAnimation(nameof(SharpBlow));
            yield return new WaitForSeconds(timeBeforeAttack);
            kaiju.brain.MakeAction();
            SendDamage(damage, kaiju);
        }

    }
}
