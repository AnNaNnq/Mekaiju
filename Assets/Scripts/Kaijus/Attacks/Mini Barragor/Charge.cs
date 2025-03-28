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
            base.Active(p_kaiju);

            if (p_kaiju as KaijuInstance != null)
            {
                KaijuInstance t_kaiju = p_kaiju as KaijuInstance;
                t_kaiju.OnCollision += HandleCollision;
                _kaiju.motor.StopKaiju();
            }
            _rb = p_kaiju.GetComponent<Rigidbody>();

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
