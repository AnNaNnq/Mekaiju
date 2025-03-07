using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class SharpBlow : Attack
    {
        [Separator]
        public float timeBeforeAttack = 1;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);
            p_kaiju.StartCoroutine(AttackEnumerator(p_kaiju));
        }

        public override IEnumerator AttackEnumerator(EntityInstance p_kaiju)
        {
            base.AttackEnumerator(p_kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)p_kaiju;
            t_kaiju.animator.AttackAnimation(nameof(SharpBlow));
            yield return new WaitForSeconds(timeBeforeAttack);
            t_kaiju.brain.MakeAction();
            SendDamage(damage, p_kaiju);
        }

    }
}
