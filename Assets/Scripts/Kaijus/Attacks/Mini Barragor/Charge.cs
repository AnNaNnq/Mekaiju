using Mekaiju.Entity;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.Attack
{
    public class Charge : Attack, ICharge
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
            ICharge t_charge = this;
            yield return t_charge.Charge(_kaiju, chargeSpeed, chargeDuration, chargePrepTime);
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
