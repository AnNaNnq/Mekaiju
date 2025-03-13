using Mekaiju.Entity;
using Mekaiju.Utils;
using MyBox;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class RollAndSlash : Attack
    {
        [Separator]
        public float attackRange = 10f;
        public float rollSpeed = 200f;

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)p_kaiju;
            _kaiju = t_kaiju;

            if (_kaiju.TargetInRange(range) && !_kaiju.TargetInRange(attackRange))
            {
                _kaiju.animator.AttackAnimation("Roll");
                _kaiju.motor.StartKaiju();
                _kaiju.motor.SetSpeed(rollSpeed);
                _kaiju.motor.MoveTo(_kaiju.target.transform.position, attackRange);
                _kaiju.StartCoroutine(RoolDuration());
            }
            else
            {
                Slash();
            }
        }

        public void Slash()
        {
            _kaiju.motor.SetSpeed(100);
            _kaiju.animator.AttackAnimation("RollAndSlash");
            _kaiju.motor.StopKaiju();
        }

        IEnumerator RoolDuration()
        {
            while (Vector3.Distance(_kaiju.transform.position, _kaiju.GetTargetPos()) > attackRange)
            {
                yield return new WaitForSeconds(0.1f);
            }
            Slash();
        }

        public override void onAction()
        {
            base.onAction();
            SendDamage(damage);
        }
    }
}
