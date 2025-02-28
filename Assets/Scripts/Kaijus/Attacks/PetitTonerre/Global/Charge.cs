using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class Charge : IAttack
    {
        [Separator]
        [OverrideLabel("Damage (% of DMG)")]
        public float damage = 50;
        [OverrideLabel("Charge Speed (Force Pulse)")]
        [Tooltip("Not % of speed")]
        public float chargeSpeed = 10;
        [OverrideLabel("Charge Duration (sec)")]
        public float chargeDuration = 0.5f;
        [OverrideLabel("Stop Distance (m)")]
        public float stopChargeDistance = 10;
        [OverrideLabel("Time Prep Before Charge (sec)")]
        public float chargePrepTime = 1;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(KaijuInstance kaiju)
        {
            base.Active(kaiju);

            kaiju.motor.StopAI();
            

            kaiju.StartCoroutine(Attack(kaiju));

        }

        public override IEnumerator Attack(KaijuInstance kaiju)
        {
            float t_time = 0;
            Vector3 t_targetPosition = kaiju.GetTargetPos();
            while (t_time < chargePrepTime)
            {
                kaiju.motor.LookTarget();
                t_targetPosition = kaiju.GetTargetPos();
                yield return new WaitForSeconds(0.01f);
                t_time += 0.01f;
            }
            kaiju.motor.enabled = false;
            float t_elapsedTime = 0f;
            Vector3 t_startPos = kaiju.transform.position;
            Vector3 t_direction = (t_targetPosition - t_startPos).normalized;
            Vector3 t_targetPos = t_targetPosition - t_direction * stopChargeDistance;

            t_targetPos.y = t_startPos.y;

            while (t_elapsedTime < chargeDuration)
            {
                kaiju.transform.position = Vector3.Lerp(t_startPos, t_targetPos, t_elapsedTime / chargeDuration);
                t_elapsedTime += Time.deltaTime;
                yield return null;
            }

            kaiju.transform.position = t_targetPos;

            kaiju.motor.enabled = true;
            kaiju.motor.StartIA();

            SendDamage(damage, kaiju);
        }
    }
}
