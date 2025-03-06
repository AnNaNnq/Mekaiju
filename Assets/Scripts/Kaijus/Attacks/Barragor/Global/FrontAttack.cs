using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class FrontAttack : IAttack
    {
        [Separator]
        public float attackRange = 10;
        [OverrideLabel("Sprint Speed (% of Speed")]
        public float sprintSpeed = 110;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            
            bool t_return = (canUse && kaiju.TargetInRange(range)) || (canUse && kaiju.TargetInRange(attackRange));
            if (otherRange > 0)
            {
                t_return &= !kaiju.TargetInRange(otherRange);
            }
            return t_return;
        }

        public override void Active(IEntityInstance kaiju)
        {
            base.Active(kaiju);

            KaijuInstance p_kaiju = (KaijuInstance)kaiju;
            if (p_kaiju.TargetInRange(range) && !p_kaiju.TargetInRange(attackRange))
            {
                Debug.Log("sprint");
                p_kaiju.motor.MoveTo(p_kaiju.target.transform.position, sprintSpeed, attackRange);
                kaiju.StartCoroutine(SprintDuration(p_kaiju));
            }
            else
            {
                AttackFront(p_kaiju);
            }
        }

        public void AttackFront(KaijuInstance kaiju)
        {
            SendDamage(damage, kaiju);
            kaiju.brain.MakeAction();
        }

        IEnumerator SprintDuration(KaijuInstance kaiju)
        {
            while(Vector3.Distance(kaiju.transform.position, kaiju.GetTargetPos()) > attackRange)
            {
                yield return new WaitForSeconds(0.1f);
            }
            AttackFront(kaiju);
        }
    }
}
