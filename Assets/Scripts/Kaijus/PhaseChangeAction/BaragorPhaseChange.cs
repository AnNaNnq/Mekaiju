using Mekaiju.Entity.Effect;
using Mekaiju.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Mekaiju.AI.PhaseAttack
{
    public class BaragorPhaseChange : PhaseAttack
    {
        private Transform _target;
        public float force = 10f; 
        public float upwardModifier = 1.5f; 
        public float diveForce = 15f; 
        public float heightThreshold = 2f;
        private Rigidbody _rb;
        private bool isDiving = false;

        public float fallSlowFactor = 0.5f;

        public override void Action()
        {
            base.Action();

            isDiving = false;

            _rb = _kaiju.GetComponent<Rigidbody>();
            _target = _kaiju.target.transform;
            JumpTowardsTarget();
            _kaiju.StartCoroutine(Jump_Update());
        }

        void JumpTowardsTarget()
        {
            if (_target == null || _rb == null)
            {
                Debug.LogWarning("Cible ou Rigidbody manquant!");
                return;
            }

            _rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        }

        IEnumerator Jump_Update()
        {
            while (true)
            {
                Debug.Log(_kaiju.transform.position.y >= heightThreshold);
                if (!isDiving && _kaiju.transform.position.y >= heightThreshold)
                {
                    Dive();
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        void Dive()
        {
            if (_target == null || _rb == null)
            {
                Debug.LogWarning("Cible ou Rigidbody manquant!");
                return;
            }

            isDiving = true;
            Vector3 t_direction = (_target.position - _kaiju.transform.position).normalized;
            _rb.linearVelocity = Vector3.zero; // Réinitialiser la vitesse avant la plongée
            _rb.linearVelocity= new Vector3(0, _rb.linearVelocity.y * fallSlowFactor, 0); // Réduire la vitesse de chute
            Debug.Log(new Vector3(t_direction.x * diveForce, 0, t_direction.z * diveForce));
            _rb.AddForce(new Vector3(t_direction.x * diveForce, 0, t_direction.z * diveForce), ForceMode.Impulse);
        }

    }
}
