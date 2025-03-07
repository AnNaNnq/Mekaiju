using MyBox;
using Mekaiju.Entity;
using UnityEngine;
using System.Collections;

namespace Mekaiju.AI.Attack
{
    public class SharpMandible : Attack
    {
        [Separator]
        public float timeBeforeAttack = 1;

        public override void Active(IEntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            p_kaiju.StartCoroutine(AttackEnumerator(p_kaiju));
        }

        public override IEnumerator AttackEnumerator(IEntityInstance p_kaiju)
        {
            base.AttackEnumerator(p_kaiju);

            KaijuInstance t_instance = (KaijuInstance)p_kaiju;

            t_instance.animator.AttackAnimation(nameof(SharpMandible));

            yield return new WaitForSeconds(timeBeforeAttack);

            SendDamage(damage, p_kaiju);
        }
    }
}
