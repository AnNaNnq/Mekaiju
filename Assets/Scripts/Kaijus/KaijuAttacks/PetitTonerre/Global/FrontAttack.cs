using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI
{
    public class FrontAttack : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        [HideInInspector]
        public float timeBeforeAttack = 1; //Sera util plus tard fait pas une syncope alex
        public float sprintRange = 30;
        [OverrideLabel("Sprint Speed (% of Speed")]
        public float sprintSpeed = 110;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            bool t_return = (canUse && kaiju.TargetInRange(range)) || (canUse && kaiju.TargetInRange(sprintRange));
            if (otherRange > 0)
            {
                t_return &= !kaiju.TargetInRange(otherRange);
            }
            return t_return;
        }

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);
            if(kaiju.TargetInRange(sprintRange))
            {
                kaiju.motor.MoveTo(kaiju.target.transform.position, sprintSpeed, range);
                kaiju.StartCoroutine(SprintDuration(kaiju));
            }
            else
            {
                AttackFront(kaiju);
            }
        }

        public void AttackFront(KaijuInstance kaiju)
        {
            Debug.Log("Attack !");
            kaiju.brain.MakeAction();

        }

        IEnumerator SprintDuration(KaijuInstance kaiju)
        {
            while(Vector3.Distance(kaiju.transform.position, kaiju.GetTargetPos()) > range)
            {
                yield return new WaitForSeconds(0.1f);
            }
            AttackFront(kaiju);
        }
    }
}
