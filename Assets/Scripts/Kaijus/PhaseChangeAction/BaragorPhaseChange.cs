using Mekaiju.Entity.Effect;
using Mekaiju.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

namespace Mekaiju.AI.PhaseAttack
{
    public class BaragorPhaseChange : PhaseAttack
    {
        private Transform _target;
        private Rigidbody _rb;

        public float chargeDuration = 1.0f;
        public float chargeSpeed = 10000;
        public float verticalBoost = 1000f;
        public int nbJump = 5;

        int _currentJump = 0;

        public override void Action()
        {
            base.Action();

            _rb = _kaiju.GetComponent<Rigidbody>();

            _kaiju.brain.isStopped = true;

            _target = _kaiju.target.transform;
            _currentJump = 0;
            _kaiju.StartCoroutine(RecursiceJump());
        }

        IEnumerator RecursiceJump()
        {
            Vector3 t_targetPosition = _kaiju.GetTargetPos();
            Vector3 t_startPos = _kaiju.transform.position;
            Vector3 t_direction = (t_targetPosition - t_startPos).normalized;

            // Appliquer une poussée verticale instantanée
            _rb.AddForce(Vector3.up * verticalBoost, ForceMode.Impulse);

            float t_elapsed = 0f;
            while (t_elapsed < chargeDuration)
            {
                _rb.AddForce(t_direction * (chargeSpeed / chargeDuration), ForceMode.Acceleration);
                t_elapsed += Time.deltaTime;
                yield return null;
            }

            _currentJump++;

            yield return new WaitForSeconds(1);

            if (_currentJump < nbJump)
            {
                _kaiju.StartCoroutine(RecursiceJump());
            }
            else
            {
                _kaiju.brain.isStopped = false;
            }
        }

    }
}
