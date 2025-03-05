using MyBox;
using Mekaiju.Entity;
using UnityEngine;
using System.Collections;

namespace Mekaiju.AI.Attack
{
    public class SharpMandible : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        public float timeBeforeAttack = 1;

        public override void Active(IEntityInstance kaiju)
        {
            base.Active(kaiju);

            kaiju.StartCoroutine(Attack(kaiju));
        }

        public override IEnumerator Attack(IEntityInstance kaiju)
        {
            base.Attack(kaiju);

            LittleKaijuInstance _instance = (LittleKaijuInstance)kaiju;

            _instance.animator.AttackAnimation(nameof(SharpMandible));

            yield return new WaitForSeconds(timeBeforeAttack);

            SendDamage(damage, kaiju);
        }
    }
}
