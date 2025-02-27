using UnityEngine;
using System.Collections;
using MyBox;

namespace Mekaiju.AI
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

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            kaiju.StartCoroutine(Attack(kaiju));
        }

        public override IEnumerator Attack(KaijuInstance kaiju)
        {
            base.Attack(kaiju);
            kaiju.animator.AttackAnimation(nameof(SnakeStrike));
            yield return new WaitForSeconds(timeBeforeAttack);
            Debug.Log($"Sharp Blow fait {damage} degats");
            kaiju.brain.MakeAction();

        }
    }
}
