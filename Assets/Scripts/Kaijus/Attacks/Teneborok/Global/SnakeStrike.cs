using UnityEngine;
using System.Collections;
using MyBox;
using Mekaiju.Entity;

namespace Mekaiju.AI.Attack
{
    public class SnakeStrike : IAttack
    {
        [Separator]
        public float timeBeforeAttack = 1;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);
            p_kaiju.StartCoroutine(Attack(p_kaiju));
        }

        public override IEnumerator Attack(EntityInstance p_kaiju)
        {
            base.Attack(p_kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)p_kaiju;

            t_kaiju.animator.AttackAnimation(nameof(SnakeStrike));
            yield return new WaitForSeconds(timeBeforeAttack);
            t_kaiju.brain.MakeAction();
            SendDamage(damage, p_kaiju);
        }
    }
}
