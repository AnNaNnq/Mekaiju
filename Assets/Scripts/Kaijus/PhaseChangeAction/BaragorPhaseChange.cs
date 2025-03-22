using Mekaiju.AI.Attack;
using Mekaiju.Entity.Effect;
using Mekaiju.Utils;
using MyBox;
using System.Collections;
using UnityEngine;

namespace Mekaiju.AI.PhaseAttack
{
    public class BaragorPhaseChange : PhaseAttack, IShockWave
    {
        private Transform _target;
        private Rigidbody _rb;

        public float chargeDuration = 1.0f;
        public float chargeSpeed = 10000;
        public float verticalBoost = 1000f;
        public int nbJump = 5;

        public float healAmountWhenFail = 50;

        int _currentJump = 0;

        [Separator("ShockWave")]
        public ShockWaveStat wave;

        public override void Action()
        {
            base.Action();

            _rb = _kaiju.GetComponent<Rigidbody>();

            _kaiju.brain.isStopped = true;

            wave.kaiju = _kaiju;

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

            IShockWave t_shockWave = this;
            t_shockWave.LaunchWave(wave, _kaiju.transform);

            if (_currentJump < nbJump)
            {
                _kaiju.StartCoroutine(RecursiceJump());
            }
            else
            {
                LaunchQTE();
            }
        }

        void LaunchQTE()
        {
            input = UnityEngine.Random.Range(0, _qte.qteInputActions.Count);

            _qte.StartQTE();
        }

        public override void Success()
        {
            base.Success();
            MechaInstance t_mecha = _kaiju.target.GetComponent<MechaInstance>();
            _kaiju.brain.isStopped = false;
            _kaiju.motor.StartKaiju();
        }

        public override void Failure()
        {
            base.Failure();

            _kaiju.StartCoroutine(charge());

            _kaiju.Heal(healAmountWhenFail);
            _kaiju.SetPhase(1);
        }

        IEnumerator charge()
        {
            Vector3 t_targetPosition = _kaiju.GetTargetPos();
            Vector3 t_startPos = _kaiju.transform.position;
            Vector3 t_direction = (t_targetPosition - t_startPos).normalized;

            float t_elapsed = 0f;
            while (t_elapsed < chargeDuration)
            {
                _rb.AddForce(t_direction * (chargeSpeed / chargeDuration), ForceMode.Acceleration);
                t_elapsed += Time.deltaTime;
                yield return null;
            }

            _kaiju.brain.isStopped = false;
        }

    }
}
