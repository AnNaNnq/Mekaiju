using MyBox;
using Mekaiju.Entity;
using UnityEngine;
using System.Collections;

namespace Mekaiju.AI.Attack
{
    public class SharpMandible : IAttack
    {
        [Separator]
        public float timeBeforeAttack = 1;

        public override void Active(IEntityInstance kaiju)
        {
            base.Active(kaiju);

            kaiju.StartCoroutine(Attack(kaiju));
        }

        public override IEnumerator Attack(IEntityInstance kaiju)
        {
            base.Attack(kaiju);

            KaijuInstance _instance = (KaijuInstance)kaiju;

            _instance.animator.AttackAnimation(nameof(SharpMandible));

            yield return new WaitForSeconds(timeBeforeAttack);

            SendDamage(damage, kaiju);
        }
    }
}
