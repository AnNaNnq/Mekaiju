using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;
namespace Mekaiju.AI.Attack
{
    public class RollDash : Attack
    {
        [Separator]
        public float attackRange = 10f;
        public float rollSpeed = 200f;
        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            if (_kaiju.TargetInRange(range) && !_kaiju.TargetInRange(attackRange))
            {
                _kaiju.animator.AttackAnimation("Roll");
                _kaiju.motor.StartKaiju();
                _kaiju.motor.SetSpeed(rollSpeed);
                _kaiju.motor.MoveTo(_kaiju.target.transform.position, attackRange);
            }
            else
            {
                //Slash();
            }
        }

        IEnumerator RoolDuration()
        {
            while (Vector3.Distance(_kaiju.transform.position, _kaiju.GetTargetPos()) > attackRange)
            {
                yield return new WaitForSeconds(0.1f);
            }
            //Slash();
        }
    }
}
