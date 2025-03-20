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
        public float chargeSpeed = 10000;
        [OverrideLabel("Time Prep Before Charge (sec)")]
        public float chargePrepTime = 1;
        public float chargeDuration = 1.0f; // Temps de charge

        Rigidbody _rb;

        public override void Active(EntityInstance p_kaiju)
        {
            KaijuInstance t_kaiju = p_kaiju as KaijuInstance;
            base.Active(t_kaiju);
            t_kaiju.OnCollision += HandleCollision;
            _rb = t_kaiju.GetComponent<Rigidbody>();

            _kaiju.motor.StopKaiju();

            _kaiju.StartCoroutine(AttackEnumerator());

        }

        public override IEnumerator AttackEnumerator()
        {
            float t_time = 0;
            Vector3 t_targetPosition = _kaiju.GetTargetPos();

            while (t_time < chargePrepTime)
            {
                _kaiju.motor.StopKaiju();
                _kaiju.motor.LookTarget();
                t_targetPosition = _kaiju.GetTargetPos();
                yield return new WaitForSeconds(0.01f);
                t_time += 0.01f;
            }

            Vector3 t_startPos = _kaiju.transform.position;
            Vector3 t_direction = (t_targetPosition - t_startPos).normalized;


            float t_elapsed = 0f;
            while (t_elapsed < chargeDuration)
            {
                _rb.AddForce(t_direction * (chargeSpeed / chargeDuration), ForceMode.Acceleration);
                t_elapsed += Time.deltaTime;
                yield return null;
            }
            OnEnd();
        }

        void HandleCollision(Collision p_collision)
        {
            if (!p_collision.collider.CompareTag("Ground") && !p_collision.collider.CompareTag("Kaiju"))
            {
                _rb.angularVelocity = Vector3.zero;
                _rb.linearVelocity = Vector3.zero;
                _kaiju.motor.StartKaiju();
            }
            if (p_collision.collider.CompareTag("Player"))
            {
                SendDamage(damage);
            }
        }
    }
}
