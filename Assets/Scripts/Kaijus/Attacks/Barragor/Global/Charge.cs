using Mekaiju.Entity;
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
        [Range(100, 300)]
        public float chargeSpeed = 10;
        [OverrideLabel("Time Prep Before Charge (sec)")]
        public float chargePrepTime = 1;

        KaijuInstance _kaiju;

        Rigidbody _rb;

        public override bool CanUse(KaijuInstance kaiju, float otherRange = 0)
        {
            return base.CanUse(kaiju, otherRange);
        }

        public override void Active(IEntityInstance kaiju)
        {
            KaijuInstance t_kaiju = kaiju as KaijuInstance;
            base.Active(t_kaiju);
            t_kaiju.OnCollision += HandleCollision;
            _rb = t_kaiju.GetComponent<Rigidbody>();
            _kaiju = t_kaiju;

            t_kaiju.motor.StopAI();


            t_kaiju.StartCoroutine(Attack(t_kaiju));

        }

        public override IEnumerator Attack(IEntityInstance kaiju)
        {
            float t_time = 0;
            KaijuInstance t_kaiju = kaiju as KaijuInstance;
            Vector3 t_targetPosition = t_kaiju.GetTargetPos();

            while (t_time < chargePrepTime)
            {
                t_kaiju.motor.LookTarget();
                t_targetPosition = t_kaiju.GetTargetPos();
                yield return new WaitForSeconds(0.01f);
                t_time += 0.01f;
            }

            t_kaiju.motor.enabled = false;
            Vector3 t_startPos = kaiju.transform.position;
            Vector3 t_direction = (t_targetPosition - t_startPos).normalized;

            _rb.AddForce(t_direction * chargeSpeed, ForceMode.Impulse);
            SendDamage(damage, kaiju);
        }

        void HandleCollision(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                SendDamage(damage, _kaiju);
            }
        }
    }
}
