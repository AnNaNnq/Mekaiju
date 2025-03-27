using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class FrontAttack : Attack
    {
        [Separator]
        public float attackRange = 10;
        [OverrideLabel("Sprint Speed (% of Speed")]
        public float sprintSpeed = 110;

        public override bool CanUse(KaijuInstance p_kaiju, float p_otherRange = 0)
        {
            bool t_return = (canUse && p_kaiju.TargetInRange(range)) || (canUse && p_kaiju.TargetInRange(attackRange));
            if (p_otherRange > 0)
            {
                t_return &= !p_kaiju.TargetInRange(p_otherRange);
            }
            return t_return;
        }

        public override void Active(EntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            if (_kaiju.TargetInRange(range) && !_kaiju.TargetInRange(attackRange))
            {
                Debug.Log("run");
                _kaiju.motor.StartKaiju();
                _kaiju.motor.SetSpeed(sprintSpeed);
                _kaiju.motor.MoveTo(_kaiju.target.transform.position, attackRange);
                _kaiju.StartCoroutine(SprintDuration(_kaiju));
            }
            else
            {
                Debug.Log("Attack");
                AttackFront(_kaiju);
            }
        }

        public void AttackFront(KaijuInstance p_kaiju)
        {
            _kaiju.animator.AttackAnimation("Sharp");
        }

        IEnumerator SprintDuration(KaijuInstance p_kaiju)
        {
            while(Vector3.Distance(p_kaiju.transform.position, p_kaiju.GetTargetPos()) > attackRange)
            {
                yield return new WaitForSeconds(0.1f);
            }
            AttackFront(p_kaiju);
        }

        public override void OnAction()
        {
            base.OnAction();

            SendDamage(damage);
        }
    }
}
