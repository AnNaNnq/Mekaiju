using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class SharpBlow : IAttack
    {
        [Separator]
        public float timeBeforeAttack = 1;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);
            p_kaiju.StartCoroutine(Attack(p_kaiju));
        }

        public override IEnumerator Attack(EntityInstance kaiju)
        {
            base.Attack(kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)kaiju;
            t_kaiju.animator.AttackAnimation(nameof(SharpBlow));
            yield return new WaitForSeconds(timeBeforeAttack);
            t_kaiju.brain.MakeAction();
            SendDamage(damage, kaiju);
        }

    }
}
