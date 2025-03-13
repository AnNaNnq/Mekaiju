using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class Charge : Attack
    {
        [Separator]
        [OverrideLabel("Charge Speed (Force Pulse)")]
        [Tooltip("Not % of speed")]
        [Range(200000, 300000)]
        public float chargeSpeed = 10;
        [OverrideLabel("Time Prep Before Charge (sec)")]
        public float chargePrepTime = 1;

        Rigidbody _rb;

        public override void Active(EntityInstance p_kaiju)
        {
            KaijuInstance t_kaiju = p_kaiju as KaijuInstance;
            base.Active(t_kaiju);
            t_kaiju.OnCollision += HandleCollision;
            _rb = t_kaiju.GetComponent<Rigidbody>();

            _kaiju.motor.StopKaiju();


            _kaiju.StartCoroutine(AttackEnumerator(_kaiju));

        }

        public override IEnumerator AttackEnumerator(EntityInstance p_kaiju)
        {
            float t_time = 0;
            Vector3 t_targetPosition = _kaiju.GetTargetPos();

            while (t_time < chargePrepTime)
            {
                _kaiju.motor.LookTarget();
                t_targetPosition = _kaiju.GetTargetPos();
                yield return new WaitForSeconds(0.01f);
                t_time += 0.01f;
            }

            //_kaiju.motor.enabled = false;
            Vector3 t_startPos = _kaiju.transform.position;
            Vector3 t_direction = (t_targetPosition - t_startPos).normalized;

            _rb.AddForce(t_direction * chargeSpeed, ForceMode.Impulse);
            SendDamage(damage, _kaiju);
        }

        void HandleCollision(Collision p_collision)
        {
            if (p_collision.collider.CompareTag("Player"))
            {
                SendDamage(damage, _kaiju);
            }
        }
    }
}
