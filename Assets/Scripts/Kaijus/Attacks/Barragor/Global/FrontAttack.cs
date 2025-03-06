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

        public override bool CanUse(KaijuInstance p_kaiju, float p_otherRange = 0)
        {
            
            bool t_return = (canUse && p_kaiju.TargetInRange(range)) || (canUse && p_kaiju.TargetInRange(attackRange));
            if (p_otherRange > 0)
            {
                t_return &= !p_kaiju.TargetInRange(p_otherRange);
            }
            return t_return;
        }

        public override void Active(IEntityInstance p_kaiju)
        {
            base.Active(p_kaiju);

            KaijuInstance t_kaiju = (KaijuInstance)p_kaiju;
            if (t_kaiju.TargetInRange(range) && !t_kaiju.TargetInRange(attackRange))
            {
                Debug.Log("sprint");
                t_kaiju.motor.MoveTo(t_kaiju.target.transform.position, sprintSpeed, attackRange);
                t_kaiju.StartCoroutine(SprintDuration(t_kaiju));
            }
            else
            {
                AttackFront(t_kaiju);
            }
        }

        public void AttackFront(KaijuInstance p_kaiju)
        {
            SendDamage(damage, p_kaiju);
            p_kaiju.brain.MakeAction();
        }

        IEnumerator SprintDuration(KaijuInstance p_kaiju)
        {
            while(Vector3.Distance(p_kaiju.transform.position, p_kaiju.GetTargetPos()) > attackRange)
            {
                yield return new WaitForSeconds(0.1f);
            }
            AttackFront(p_kaiju);
        }
    }
}
