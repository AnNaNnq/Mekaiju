using UnityEngine;
using System.Collections;
using MyBox;
using Mekaiju.Entity;

namespace Mekaiju.AI.Attack
{
    public class SnakeStrike : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        public float timeBeforeAttack = 1;

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

            t_kaiju.animator.AttackAnimation(nameof(SnakeStrike));
            yield return new WaitForSeconds(timeBeforeAttack);
            t_kaiju.brain.MakeAction();
            SendDamage(damage, kaiju);
        }
    }
}
